using BussinessObjects;
using BussinessObjects.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Services.Interfaces;

namespace PRN___Final_Project.Pages
{
    public class CheckInModel : PageModel
    {
        private readonly IAirPlaneService _airplaneService;
        private readonly ITicketService _ticketService;
        private readonly ClassTypesConfig _classTypesConfig;
        private readonly TicketTypesConfig _ticketTypesConfig;

        public ClassTypesConfig ClassTypesConfig => _classTypesConfig;
        public TicketTypesConfig TicketTypesConfig => _ticketTypesConfig;

        public CheckInModel(IAirPlaneService airplaneService, ITicketService ticketService, IOptions<ClassTypesConfig> classTypesConfig, IOptions<TicketTypesConfig> ticketTypesConfig)
        {
            _airplaneService = airplaneService;
            _ticketService = ticketService;
            _classTypesConfig = classTypesConfig.Value;
            _ticketTypesConfig = ticketTypesConfig.Value;
        }

        [BindProperty]
        public AirPlane AirPlane { get; set; }

        public int TotalRows { get; set; }
        public int TotalSeats { get; set; }
        public int AllowedSeats { get; set; }
        public string AllowedClassType { get; set; }

        [BindProperty]
        public List<string> SelectedSeats { get; set; }
        [BindProperty]
        public List<string> BookedSeats { get; set; }

        public async Task OnGetAsync(int planeId)
        {
            AirPlane = await _airplaneService.GetAirPlaneByIdAsync(planeId);
            TotalSeats = AirPlane.VipSeatNumber + AirPlane.NormalSeatNumber;
            TotalRows = (int)Math.Ceiling(TotalSeats / 6.0);
            var flightType = HttpContext.Session.GetString("FlightType");
            var bookingData = HttpContext.Session.GetObjectFromJson<Booking>("BookingData");
            AllowedSeats = bookingData.AdultNum + bookingData.ChildNum + bookingData.BabyNum;
            
            if (flightType == _ticketTypesConfig.OutBoundFlight)
            {
                AllowedClassType = bookingData.ClassType;
            }
            else
            {
                AllowedClassType = bookingData.ReturnClassType;
            }

            SelectedSeats = new List<string>();
            BookedSeats = await _ticketService.GetBookedSeatsByFlightIdAsync(bookingData.FlightId, flightType);
        }

        public async Task<IActionResult> OnPostAsync(decimal carryLuggage, decimal baggage)
        {
            var bookingData = HttpContext.Session.GetObjectFromJson<Booking>("BookingData");
            var ticketType = HttpContext.Session.GetString("FlightType");

            // Split the SelectedSeats string into a list
            SelectedSeats = Request.Form["SelectedSeats"].ToString().Split(',').ToList();

            foreach (var seat in SelectedSeats)
            {
                // Determine ClassType based on the first character of the seat number
                var classType = seat.StartsWith("V") ? _classTypesConfig.Business : _classTypesConfig.Economy;

                // Remove the first character to get the actual seat number
                var actualSeatNumber = seat.Substring(1);

                var ticket = new Ticket
                {
                    SeatNumber = actualSeatNumber, // Save the actual seat number
                    TicketType = ticketType,
                    IssuedDate = DateTime.Now,
                    ClassType = classType,
                    Carryluggage = carryLuggage,
                    Baggage = baggage,
                    BookingId = bookingData.BookingId
                };

                await _ticketService.CreateTicketAsync(ticket);
            }

            if (ticketType == _ticketTypesConfig.OutBoundFlight)
            {
                return RedirectToPage("/TicketDetails", new { bookingId = bookingData.BookingId, isOutbound = true });
            }

            else
            {
                return RedirectToPage("/TicketDetails", new { bookingId = bookingData.BookingId, isOutbound = false });
            }
        }
    }
}
