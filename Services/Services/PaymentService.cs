using BussinessObjects;
using BussinessObjects.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Repositories;
using Repositories.Interface;
using Services.Interfaces;
using Services.vnpay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly IBookingService _bookingService;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService( IConfiguration configuration, IBookingService bookingService, IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _bookingService = bookingService;
            _unitOfWork = unitOfWork;

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
            string vnpHashSecret = _configuration["VnpaySettings:vnp_HashSecret"]!;
            string inputHash = vnpay.GetResponseData("vnp_SecureHash");
            bool isValidSignature = vnpay.ValidateSignature(inputHash, vnpHashSecret);
            if (!isValidSignature)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.BADREQUEST, "Chữ ký không hợp lệ!");
            }
            string responseCode = vnpay.GetResponseData("vnp_ResponseCode");
            if (responseCode == "00")
            {
                int bookingId = int.Parse(vnpay.GetResponseData("vnp_TxnRef"));
                var booking = await _bookingService.GetBookingByIdAsync(bookingId);
                if (booking == null)
                {
                    throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, "Không tìm thấy booking.");
                }

                Payment payment = new Payment
                {
                    BookingId = booking.BookingId,
                    UserId = booking.UserId,
                    Amount = booking.TotalPrice,
                    PaymentDate = DateTime.Now
                };
                booking.PaymentStatus = "Success";
                await CreatePaymentAsync(payment);
                await _bookingService.UpdateBookingAsync(booking);

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
                booking.PaymentStatus = "Failed";
                await _bookingService.UpdateBookingAsync(booking);
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
                return await _unitOfWork.Repository<Payment>().GetAllAsync();
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
