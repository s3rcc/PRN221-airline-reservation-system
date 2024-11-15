using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using BussinessObjects;
using DataAccessObjects;
using Services.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using BussinessObjects.Config;
using Microsoft.Extensions.Options;

namespace PRN___Final_Project.Pages.CRUD.BookingManager
{
    public class CreateModel : PageModel
    {
        private readonly IBookingService _bookingService;
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;
        private readonly PaymentStatusConfig _paymentStatusConfig;
        private readonly ITierService _tierService;
        private readonly ILocationService _locationService;
        public CreateModel(IBookingService bookingService, IUserService userService, UserManager<User> userManager, IOptions<PaymentStatusConfig> paymentStatusConfig, ITierService tierService, ILocationService locationService)
        {
            _bookingService = bookingService;
            _userService = userService;
            _userManager = userManager;
            Booking = new Booking();
            _paymentStatusConfig = paymentStatusConfig.Value;
            _tierService = tierService;
            _locationService = locationService;
        }

        [BindProperty]
        public Booking Booking { get; set; } = default!;
        [BindProperty]
        public string Tier { get; set; } = default!;
        [BindProperty]
        public decimal Discount { get; set; } = default!;
        [BindProperty]
        public decimal PriceAfterDiscount { get; set; } = default!;
        [BindProperty]
        public string UserName { get; set; } = default!;
        [BindProperty]
        public string OriginLocation { get; set; } = default!;
        [BindProperty]
        public string DestinationLocation { get; set; } = default!;
        [BindProperty]
        public string FlightType { get; set; } = default!;

        public FlightData FlightData { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            FlightData = HttpContext.Session.GetObjectFromJson<FlightData>("FlightData");
            if (FlightData == null)
            {
                return RedirectToPage("/Errors/404");
            }

            // Kiểm tra xem user đã đăng nhập chưa
            if (!User.Identity.IsAuthenticated)
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để tiếp tục.";
                return Redirect($"/Login?returnUrl=/CRUD/BookingManager/Create");
            }

            // Kiểm tra role của user
            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user);
            var tier = await _tierService.GetTierByUserIdAsync(user.Id);
            var originLocation = await _locationService.GetLocationByIdAsync(FlightData.OriginId);
            var destinationLocation = await _locationService.GetLocationByIdAsync(FlightData.DestinationId);

            if (roles.Contains("Admin") || roles.Contains("Staff"))
            {
                // Nếu user là Admin hoặc Staff, chuyển hướng đến dashboard
                return RedirectToPage("/Dashboard");
            }
            else if (!roles.Contains("Member"))
            {
                // Nếu user không phải là Member, chặn quyền truy cập
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập trang này.";
                return RedirectToPage("/Errors/403");
            }

            Tier = tier?.TierName;
            Discount = tier?.Discount ?? 0;
            PriceAfterDiscount = FlightData.TotalPrice * (1 - Discount);
            UserName = user.UserName;
            OriginLocation = originLocation.LocationName;
            DestinationLocation = destinationLocation.LocationName;

            if (FlightData.IsOneWay)
            {
                FlightType = "One Way";
            }
            else
            {
                FlightType = "Round Trip";
            }

            TempData.Remove("ErrorMessage");
            Booking.FlightId = FlightData.OutboundFlightId;
            Booking.ReturnFlightId = FlightData.ReturnFlightId;
            Booking.TotalPrice = FlightData.TotalPrice;
            Booking.BookingDate = DateTime.Now;
            Booking.AdultNum = FlightData.AdultNum;
            Booking.ChildNum = FlightData.ChildNum;
            Booking.BabyNum = FlightData.BabyNum;
            Booking.UserId = user.Id;
            Booking.PaymentStatus = _paymentStatusConfig.Unpaid;
            Booking.Status = true;
            Booking.ClassType = FlightData.ClassType;
            Booking.ReturnClassType = FlightData.ReturnClassType;

            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            HttpContext.Session.Remove("FlightData");
            Booking.TotalPrice = PriceAfterDiscount / 1000;
            Booking.BookingId = await _bookingService.CreateBookingAsync(Booking);
            List<TicketData> ticketDatas = HttpContext.Session.GetObjectFromJson<List<TicketData>>("TicketData");

            if (Booking.BookingId > 0)
            {
                foreach (var ticketData in ticketDatas)
                {
                    ticketData.BookingId = Booking.BookingId;
                }

                HttpContext.Session.SetObjectAsJson("TicketData", ticketDatas);

                return RedirectToPage("/Payment", new { id = Booking.BookingId });
            }

            ModelState.AddModelError(string.Empty, "Không thể tạo booking. Vui lòng thử lại.");
            return Page();
        }

    }

}
