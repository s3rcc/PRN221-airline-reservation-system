using BussinessObjects;
using BussinessObjects.Config;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Services.Interfaces;

namespace PRN___Final_Project.Pages
{
    public class BookingCheckModel : PageModel
    {
        private readonly IFlightService _flightService;
        private readonly IBookingService _bookingService;
        private readonly ITicketService _ticketService;
        private readonly TicketTypesConfig _ticketTypesConfig;
        private readonly UserManager<User> _userManager;

        public BookingCheckModel(IFlightService flightService, IBookingService bookingService, ITicketService ticketService, IOptions<TicketTypesConfig> ticketTypesConfig, UserManager<User> userManager)
        {
            _flightService = flightService;
            _bookingService = bookingService;
            _ticketService = ticketService;
            _ticketTypesConfig = ticketTypesConfig.Value;
            _userManager = userManager;
        }

        [BindProperty]
        public int BookingId { get; set; }

        [BindProperty]
        public string PassengerName { get; set; }

        [BindProperty]
        public bool IsOutboundFlight { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Check if the user is authenticated
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect("/Login");
            }

            if (!User.IsInRole("Member"))
            {
                return RedirectToPage("/Errors/404");
            }


            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect("/Login");
            }

            if (!User.IsInRole("Member"))
            {
                return RedirectToPage("/Errors/404");
            }

            var booking = await _bookingService.GetBookingByIdAsync(BookingId);
            var user = _userManager.GetUserAsync(User);

            if (booking == null)
            {
                ModelState.AddModelError(string.Empty, "Booking not found.");
                return Page();
            }


            if (booking.UserId != user.Id.ToString())
            {
                ModelState.AddModelError(string.Empty, "This booking is not belong to this user.");
                return Page();
            }

            int? flightId = IsOutboundFlight ? booking.FlightId : booking.ReturnFlightId;

            if (flightId == null)
            {
                ModelState.AddModelError(string.Empty, "Flight not found for the selected option.");
                return Page();
            }

            var existingTicket = await _ticketService.GetTicketByBookingIdAndTypeAsync(BookingId, IsOutboundFlight);

            if (existingTicket.Count() > 0)
            {
                ModelState.AddModelError(string.Empty, $"Cannot check in, a ticket of type '{(IsOutboundFlight ? "Outbound" : "Return")}' already exists.");
                return Page();
            }

            if (booking.PaymentStatus == "UnPaid")
            {
                ModelState.AddModelError(string.Empty, $"Cannot check in, please pay for your booking.");
                return Page();
            }

            var flight = await _flightService.GetFlightByIdAsync(flightId.Value);
            if (flight == null || flight.PlaneId == 0)
            {
                ModelState.AddModelError(string.Empty, "Plane not found for the selected flight.");
                return Page();
            }

            HttpContext.Session.SetObjectAsJson("BookingData", booking);
            HttpContext.Session.SetString("FlightType", IsOutboundFlight ? _ticketTypesConfig.OutBoundFlight : _ticketTypesConfig.ReturnFlight);

            return RedirectToPage("/CheckIn", new { planeId = flight.PlaneId });
        }
    }
}
