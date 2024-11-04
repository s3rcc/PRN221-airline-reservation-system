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

namespace PRN___Final_Project.Pages.CRUD.BookingManager
{
    public class CreateModel : PageModel
    {
        private readonly IBookingService _bookingService;
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;

        public CreateModel(IBookingService bookingService, IUserService userService, UserManager<User> userManager)
        {
            _bookingService = bookingService;
            _userService = userService;
            _userManager = userManager;
            Booking = new Booking();
        }

        [BindProperty]
        public Booking Booking { get; set; } = default!;

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

            TempData.Remove("ErrorMessage");
            Booking.FlightId = FlightData.OutboundFlightId;
            Booking.ReturnFlightId = FlightData.ReturnFlightId;
            Booking.TotalPrice = FlightData.TotalPrice;
            Booking.BookingDate = DateTime.Now;
            Booking.AdultNum = FlightData.AdultNum;
            Booking.ChildNum = FlightData.ChildNum;
            Booking.BabyNum = FlightData.BabyNum;
            Booking.UserId = user.Id;
            Booking.PaymentStatus = "UnPaid";
            Booking.Status = true;
            Booking.ClassType = FlightData.ClassType;
            Booking.ReturnClassType = FlightData.ReturnClassType;

            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            HttpContext.Session.Remove("FlightData");
            Booking.BookingId = await _bookingService.CreateBookingAsync(Booking);
            if (Booking.BookingId > 0)
            {
                return RedirectToPage("/Payment", new { id = Booking.BookingId });
            }
            ModelState.AddModelError(string.Empty, "Không thể tạo booking. Vui lòng thử lại.");
            return Page();

        }
    }

}
