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
            var user = await _userManager.GetUserAsync(User);
            if (!User.Identity.IsAuthenticated)
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để tiếp tục.";
                return Redirect($"/Login?returnUrl=/CRUD/BookingManager/Create");
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
