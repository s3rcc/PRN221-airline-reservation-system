using BussinessObjects.Config;
using BussinessObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Services.Interfaces;
using Services.Services;

namespace PRN___Final_Project.Pages.CRUD.BookingManager
{
    public class ChooseOutboundFlightSeatModel : PageModel
    {
        private readonly IAirPlaneService _airplaneService;
        private readonly ITicketService _ticketService;
        private readonly UserManager<User> _userManager;
        private readonly ClassTypesConfig _classTypesConfig;
        private readonly TicketTypesConfig _ticketTypesConfig;
        private readonly IBookingService _bookingService;
        private readonly IFlightService _flightService;

        public ClassTypesConfig ClassTypesConfig => _classTypesConfig;
        public TicketTypesConfig TicketTypesConfig => _ticketTypesConfig;

        public ChooseOutboundFlightSeatModel(IAirPlaneService airplaneService, ITicketService ticketService, IOptions<ClassTypesConfig> classTypesConfig, IOptions<TicketTypesConfig> ticketTypesConfig, UserManager<User> userManager, IBookingService bookingService, IFlightService flightService)
        {
            _airplaneService = airplaneService;
            _ticketService = ticketService;
            _classTypesConfig = classTypesConfig.Value;
            _ticketTypesConfig = ticketTypesConfig.Value;
            _userManager = userManager;
            _bookingService = bookingService;
            _flightService = flightService;
        }

        [BindProperty]
        public AirPlane AirPlane { get; set; }
        public int TotalRows { get; set; }
        public int TotalSeats { get; set; }
        public int AllowedSeats { get; set; }
        public string AllowedClassType { get; set; }
        [BindProperty]
        public List<string> SelectedSeats { get; set; } = new List<string>();
        [BindProperty]
        public List<string> BookedSeats { get; set; }
        [BindProperty]
        public List<string> PassengerNames { get; set; }

        public async Task<IActionResult> OnGetAsync(int planeId)
        {
            var bookingId = HttpContext.Session.GetInt32("BookingId");
            var user = await _userManager.GetUserAsync(User);
            var flightData = HttpContext.Session.GetObjectFromJson<FlightData>("FlightData");
            List<TicketData> ticketDatas = HttpContext.Session.GetObjectFromJson<List<TicketData>>("TicketData");

            if (user == null || !await _userManager.IsInRoleAsync(user, "Member"))
            {
                return RedirectToPage("/Errors/404");
            }

            AirPlane = await _airplaneService.GetAirPlaneByIdAsync(planeId);
            TotalSeats = AirPlane.VipSeatNumber + AirPlane.NormalSeatNumber;
            TotalRows = (int)Math.Ceiling(TotalSeats / 6.0);
            PassengerNames = ticketDatas.Select(ticket => ticket.CustomerName).ToList();

            if (bookingId > 0)
            {
                var booking = _bookingService.GetBookingByIdAsync(bookingId.Value).Result;
                AllowedSeats = booking.AdultNum + booking.ChildNum + booking.BabyNum;
                AllowedClassType = booking.ClassType;
                BookedSeats = await _ticketService.GetBookedSeatsByFlightIdAsync(booking.FlightId, _ticketTypesConfig.OutBoundFlight);
                return Page();
            }

            AllowedSeats = flightData.AdultNum + flightData.ChildNum + flightData.BabyNum;
            AllowedClassType = flightData.ClassType;
            BookedSeats = await _ticketService.GetBookedSeatsByFlightIdAsync(flightData.OutboundFlightId, _ticketTypesConfig.OutBoundFlight);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var ticketType = _ticketTypesConfig.OutBoundFlight;
            var user = await _userManager.GetUserAsync(User);
            var bookingId = HttpContext.Session.GetInt32("BookingId");
            var flightData = HttpContext.Session.GetObjectFromJson<FlightData>("FlightData");

            List<TicketData> ticketDatas = HttpContext.Session.GetObjectFromJson<List<TicketData>>("TicketData");
            SelectedSeats = Request.Form["SelectedSeats"].ToString().Split(',').ToList();

            var tickets = new List<Ticket>();

            for (int i = 0; i < SelectedSeats.Count; i++)
            {
                var seat = SelectedSeats[i];
                var ticketData = ticketDatas[i];
                var classType = seat.StartsWith("V") ? _classTypesConfig.Business : _classTypesConfig.Economy;
                var actualSeatNumber = seat.Substring(1);

                var ticket = new Ticket
                {
                    SeatNumber = actualSeatNumber,
                    CustomerName = ticketData.CustomerName,
                    TicketType = ticketType,
                    IssuedDate = DateTime.UtcNow,
                    ClassType = classType,
                    Carryluggage = ticketData.Carryluggage,
                    Baggage = ticketData.Baggage,
                };

                tickets.Add(ticket);
            }

            HttpContext.Session.SetObjectAsJson("TicketData", tickets);


            if (bookingId > 0)
            {
                var booking = _bookingService.GetBookingByIdAsync(bookingId.Value).Result;
                if (booking.ReturnFlightId != null)
                {
                    return RedirectToPage("./ChooseReturnFlightSeat", new { planeId = booking.Flight.PlaneId });
                }

                return RedirectToPage("/Payment", new { id = bookingId });
            }

            if (flightData.IsOneWay)
            {
                return Redirect("/create-booking");
            }

            var flight = await _flightService.GetFlightByIdAsync(flightData.ReturnFlightId.Value);

            return RedirectToPage("./ChooseReturnFlightSeat", new { planeId = flight.PlaneId });
        }
    }
}

