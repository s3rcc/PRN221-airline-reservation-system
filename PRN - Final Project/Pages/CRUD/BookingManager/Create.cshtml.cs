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
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                HttpContext.Session.Clear();
                return Redirect("/login/");
            }
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

            await _bookingService.CreateBookingAsync(Booking);
            HttpContext.Session.Remove("FlightData");
            return RedirectToPage("./Index");
        }
    }
}
