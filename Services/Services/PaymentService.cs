using BussinessObjects;
using BussinessObjects.Config;
using BussinessObjects.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NuGet.Protocol.Core.Types;
using Repositories;
using Repositories.Interface;
using Services.Interfaces;
using Services.vnpay;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly IBookingService _bookingService;
        private readonly IFlightService _flightService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ClassTypesConfig _classTypesConfig;
        private readonly PaymentStatusConfig _paymentStatusConfig;

        public PaymentService( IConfiguration configuration, IBookingService bookingService, IUnitOfWork unitOfWork, IFlightService flightService, IOptions<ClassTypesConfig> classTypesConfig, IOptions<PaymentStatusConfig> paymentStatusConfig)
        {
            _configuration = configuration;
            _bookingService = bookingService;
            _unitOfWork = unitOfWork;
            _flightService = flightService;
            _classTypesConfig = classTypesConfig.Value;
            _paymentStatusConfig = paymentStatusConfig.Value;
        }
        public string CreatePaymentUrl(HttpContext context,Booking booking)
        {
            VnPayLibrary vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", _configuration["VnpaySettings:vnp_TmnCode"]!);
            vnpay.AddRequestData("vnp_Amount", ((int)(booking.TotalPrice * 100000)).ToString());  // Số tiền thanh toán
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(context));
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", $"DUNG AIRLINE thanh toan cho BookingID {booking.BookingId.ToString()}");
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_TxnRef", booking.BookingId.ToString());  // Mã giao dịch
            vnpay.AddRequestData("vnp_ExpireDate", DateTime.Now.AddMinutes(15).ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_ReturnUrl", _configuration["VnpaySettings:vnp_Returnurl"]!);  // URL callback sau khi thanh toán

            string vnpUrl = _configuration["VnpaySettings:vnp_Url"]!;
            string vnpHashSecret = _configuration["VnpaySettings:vnp_HashSecret"]!;
            booking.PaymentStatus = "Processing";   
            string paymentUrl = vnpay.CreateRequestUrl(vnpUrl, vnpHashSecret);
            return paymentUrl;
        }

        public async Task ExecutePayment(IQueryCollection queryParameters)
        {
            VnPayLibrary vnpay = new VnPayLibrary();

            foreach (var key in queryParameters.Keys)
            {
                vnpay.AddResponseData(key, queryParameters[key]!);

            }
            // Validate the VNPAY signature
            string vnpHashSecret = _configuration["VnpaySettings:vnp_HashSecret"]!;
            string inputHash = vnpay.GetResponseData("vnp_SecureHash");
            bool isValidSignature = vnpay.ValidateSignature(inputHash, vnpHashSecret);
            if (!isValidSignature)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.BADREQUEST, "Chữ ký không hợp lệ!");
            }
            string responseCode = vnpay.GetResponseData("vnp_ResponseCode");
            if (responseCode == "00") // Successful payment
            {
                int bookingId = int.Parse(vnpay.GetResponseData("vnp_TxnRef"));
                var booking = await _bookingService.GetBookingByIdAsync(bookingId);
                

                if (booking == null)
                {
                    throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, "Không tìm thấy booking.");
                }

                // Retrieve the flight and validate available seats
                var flight = await _flightService.GetFlightByIdAsync(booking.FlightId);
                int requiredSeats = booking.AdultNum + booking.ChildNum + booking.BabyNum;

                if (booking.ClassType.Equals(_classTypesConfig.Economy) && flight.AvailableNormalSeat < requiredSeats)
                {
                    throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.BADREQUEST, "Không đủ ghế thường.");
                }
                if (booking.ClassType.Equals(_classTypesConfig.Business) && flight.AvailableVipSeat < requiredSeats)
                {
                    throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.BADREQUEST, "Không đủ ghế VIP.");
                }

                // Check for return flight seat availability if applicable
                if (booking.ReturnFlightId != null)
                {
                    var returnFlight = await _flightService.GetFlightByIdAsync(booking.ReturnFlightId.Value);
                    if (booking.ReturnClassType.Equals(_classTypesConfig.Economy) && returnFlight.AvailableNormalSeat < requiredSeats)
                    {
                        throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.BADREQUEST, "Không đủ ghế thường cho chuyến bay về.");
                    }
                    if (booking.ReturnClassType.Equals(_classTypesConfig.Business) && returnFlight.AvailableVipSeat < requiredSeats)
                    {
                        throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.BADREQUEST, "Không đủ ghế VIP cho chuyến bay về.");
                    }
                }

                Payment payment = new Payment
                {
                    BookingId = booking.BookingId,
                    UserId = booking.UserId,
                    Amount = booking.TotalPrice,
                    PaymentDate = DateTime.Now
                };

                booking.PaymentStatus = _paymentStatusConfig.Paid;
                await CreatePaymentAsync(payment);
                await _bookingService.UpdateBookingAsync(booking);

                // Reduce seat for flight
                if (booking.ClassType.Equals(_classTypesConfig.Economy))
                {
                    flight.AvailableNormalSeat -= requiredSeats;
                }
                else
                {
                    flight.AvailableVipSeat -= requiredSeats;
                }
                await _flightService.UpdateFlightAsync(flight);
                // Reduce seat for return flight
                if (booking.ReturnFlightId != null)
                {

                    var returnFlight = await _flightService.GetReturnFlightByIdAsync(booking.ReturnFlightId);
                    if(booking.ReturnClassType.Equals(_classTypesConfig.Economy))
                    {
                        returnFlight.AvailableNormalSeat -= requiredSeats;
                    }
                    else
                    {
                        returnFlight.AvailableVipSeat -= requiredSeats;
                    }
                    await _flightService.UpdateFlightAsync(returnFlight);
                }
                await _unitOfWork.SaveChangeAsync();

            }
            else
            {

                //Thanh toán thất bại, cập nhật Booking
                int bookingId = int.Parse(vnpay.GetResponseData("vnp_TxnRef"));
                var booking = await _bookingService.GetBookingByIdAsync(bookingId);
                if (booking == null)
                {
                    throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, "Không tìm thấy booking.");
                }
                booking.PaymentStatus = _paymentStatusConfig.Unpaid;
                await _bookingService.UpdateBookingAsync(booking);
                await _unitOfWork.SaveChangeAsync();
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.FAILED, $"Thanh toán không thành công, mã lỗi: {responseCode}");
            }

        }
        public async Task CreatePaymentAsync(Payment payment)
        {
            try
            {
                await _unitOfWork.Repository<Payment>().AddAsync(payment);
                await _unitOfWork.SaveChangeAsync();
            }
            catch
            {
                throw new ErrorException(StatusCodes.Status500InternalServerError, ErrorCode.INTERNAL_SERVER_ERROR, "Error creating payment.");
            }
        }
        public async Task UpdatePaymentAsync(Payment payment)
        {
            try
            {
                var existingBooking = await _unitOfWork.Repository<Payment>().GetByIdAsync(payment.PaymentId) ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, "Payment not found.");
                await _unitOfWork.Repository<Payment>().UpdateAsync(payment);
                await _unitOfWork.SaveChangeAsync();
            }
            catch
            {
                throw new ErrorException(StatusCodes.Status500InternalServerError, ErrorCode.INTERNAL_SERVER_ERROR, "Error updating booking.");
            }
        }

        public async Task<IEnumerable<Payment>> GetAllPayments()
        {
            try
            {
                return await _unitOfWork.Repository<Payment>().GetAllAsync(orderBy: x => x.OrderByDescending(x => x.PaymentDate), includes: x => x.User);
            }
            catch
            {
                throw new ErrorException(StatusCodes.Status500InternalServerError, ErrorCode.INTERNAL_SERVER_ERROR, "Error getting payment.");
            }
        }


        public async Task<Payment> GetPaymentByUserId(string userId)
        {
            try
            {
                return await _unitOfWork.Repository<Payment>().FirstOrDefaultAsync(x => x.UserId == userId);
            }
            catch
            {
                throw new ErrorException(StatusCodes.Status500InternalServerError, ErrorCode.INTERNAL_SERVER_ERROR, "Error getting payment.");
            }
        }

        public async Task<IEnumerable<Payment>> GetPayments(int year)
        {
            try
            {
                return await _unitOfWork.Repository<Payment>().FindAsync(x => x.PaymentDate.Year == year);
            }
            catch
            {
                throw new ErrorException(StatusCodes.Status500InternalServerError, ErrorCode.INTERNAL_SERVER_ERROR, "Error getting payment.");
            }
        }

        public async Task<IEnumerable<Payment>> GetPayments(DateTime startDate, DateTime endDate)
        {
            try
            {
                return await _unitOfWork.Repository<Payment>().FindAsync(x => x.PaymentDate >= startDate.Date && x.PaymentDate <= endDate.Date);
            }
            catch
            {
                throw new ErrorException(StatusCodes.Status500InternalServerError, ErrorCode.INTERNAL_SERVER_ERROR, "Error getting payments.");
            }
        }

		public async Task<decimal> GetRevenue()
		{
            try
            {
                var payment = await _unitOfWork.Repository<Payment>().GetAllAsync();
				decimal totalRevenue = payment.Sum(p => p.Amount);
                return totalRevenue;
			}
            catch
            {
                throw new ErrorException(StatusCodes.Status500InternalServerError, ErrorCode.INTERNAL_SERVER_ERROR, "Error revenue.");
			}
		}
	}
}
