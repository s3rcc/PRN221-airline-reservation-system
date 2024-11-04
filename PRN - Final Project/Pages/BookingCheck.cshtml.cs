using BussinessObjects.Config;
using Microsoft.AspNetCore.Http;
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

        public BookingCheckModel(IFlightService flightService, IBookingService bookingService, ITicketService ticketService, IOptions<TicketTypesConfig> ticketTypesConfig)
        {
            _flightService = flightService;
            _bookingService = bookingService;
            _ticketService = ticketService;
            _ticketTypesConfig = ticketTypesConfig.Value;
        }

        [BindProperty]
        public int BookingId { get; set; }

        [BindProperty]
        public bool IsOutboundFlight { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var booking = await _bookingService.GetBookingByIdAsync(BookingId);
            if (booking == null)
            {
                ModelState.AddModelError(string.Empty, "Booking not found.");
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

            //if (booking.PaymentStatus == "UnPaid")
            //{
            //    ModelState.AddModelError(string.Empty, $"Cannot check in, please pay for your booking.");
            //    return Page();
            //}

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
