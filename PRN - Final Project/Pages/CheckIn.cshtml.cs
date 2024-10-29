using BussinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interfaces;

namespace PRN___Final_Project.Pages
{
    public class CheckInModel : PageModel
    {
        private readonly IAirPlaneService _airplaneService;
        private readonly ITicketService _ticketService;

        public CheckInModel(IAirPlaneService airplaneService, ITicketService ticketService)
        {
            _airplaneService = airplaneService;
            _ticketService = ticketService;
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
            
            if (flightType == "OutBoundFlight")
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
                var classType = seat.StartsWith("V") ? "Business" : "Economy";

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

            if (ticketType == "OutBoundFlight")
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
