using BussinessObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interfaces;

namespace PRN___Final_Project.Pages.CRUD.BookingManager
{
    public class BookingDetailModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly IFlightService _flightService;
        private readonly IBookingService _bookingService;

        public BookingDetailModel(UserManager<User> userManager, IFlightService flightService, IBookingService bookingService)
        {
            _userManager = userManager;
            _flightService = flightService;
            _bookingService = bookingService;
        }

        public int AdultNum { get; set; }
        public int ChildNum { get; set; }
        public int BabyNum { get; set; }
        public string UserName { get; set; }
        public List<TicketData> TicketDatas { get; set; } = new List<TicketData>();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var flightData = HttpContext.Session.GetObjectFromJson<FlightData>("FlightData");

            if (id > 0)
            {
                var booking = _bookingService.GetBookingByIdAsync(id).Result;
                HttpContext.Session.SetInt32("BookingId", id);
                AdultNum = booking.AdultNum;
                ChildNum = booking.ChildNum;
                BabyNum = booking.BabyNum;
            }
            
            var user = await _userManager.GetUserAsync(User);
            if (user == null || !await _userManager.IsInRoleAsync(user, "Member"))
            {
                return RedirectToPage("/Errors/404");
            }
            if (user != null)
            {
                UserName = user.UserName;
            }

            if (flightData != null)
            {
                AdultNum = flightData.AdultNum;
                ChildNum = flightData.ChildNum;
                BabyNum = flightData.BabyNum;
            }
            return Page();  
        }

        public async Task<IActionResult> OnPostAsync(List<TicketData> ticketData)
        {
            HttpContext.Session.SetObjectAsJson("TicketData", ticketData);
            var bookingId = HttpContext.Session.GetInt32("BookingId");

            if (bookingId > 0)
            {
                var booking = await _bookingService.GetBookingByIdAsync(bookingId.Value);
                return RedirectToPage("/CRUD/BookingManager/ChooseOutboundFlightSeat", new { planeId = booking.Flight.PlaneId });
            }

            var flightData = HttpContext.Session.GetObjectFromJson<FlightData>("FlightData");
            var flight = await _flightService.GetFlightByIdAsync(flightData.OutboundFlightId);
            return RedirectToPage("/CRUD/BookingManager/ChooseOutboundFlightSeat", new { planeId = flight.PlaneId });
        }
    }
}
