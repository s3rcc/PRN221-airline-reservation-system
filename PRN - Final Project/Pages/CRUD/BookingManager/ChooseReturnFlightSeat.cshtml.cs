using BussinessObjects.Config;
using BussinessObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Services.Interfaces;

namespace PRN___Final_Project.Pages.CRUD.BookingManager
{
    public class ChooseReturnFlightSeatModel : PageModel
    {
        private readonly IAirPlaneService _airplaneService;
        private readonly ITicketService _ticketService;
        private readonly UserManager<User> _userManager;
        private readonly ClassTypesConfig _classTypesConfig;
        private readonly TicketTypesConfig _ticketTypesConfig;
        private readonly IBookingService _bookingService;

        public ClassTypesConfig ClassTypesConfig => _classTypesConfig;
        public TicketTypesConfig TicketTypesConfig => _ticketTypesConfig;

        public ChooseReturnFlightSeatModel(IAirPlaneService airplaneService, ITicketService ticketService, IOptions<ClassTypesConfig> classTypesConfig, IOptions<TicketTypesConfig> ticketTypesConfig, UserManager<User> userManager, IBookingService bookingService)
        {
            _airplaneService = airplaneService;
            _ticketService = ticketService;
            _classTypesConfig = classTypesConfig.Value;
            _ticketTypesConfig = ticketTypesConfig.Value;
            _userManager = userManager;
            _bookingService = bookingService;
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
                AllowedClassType = booking.ReturnClassType;
                BookedSeats = await _ticketService.GetBookedSeatsByFlightIdAsync(booking.ReturnFlightId.Value, _ticketTypesConfig.ReturnFlight);
                return Page();
            }

            AllowedSeats = flightData.AdultNum + flightData.ChildNum + flightData.BabyNum;
            AllowedClassType = flightData.ReturnClassType;
            BookedSeats = await _ticketService.GetBookedSeatsByFlightIdAsync(flightData.ReturnFlightId.Value, _ticketTypesConfig.ReturnFlight);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var ticketType = _ticketTypesConfig.ReturnFlight;
            var user = await _userManager.GetUserAsync(User);
            var bookingId = HttpContext.Session.GetInt32("BookingId");

            var existingTickets = HttpContext.Session.GetObjectFromJson<List<TicketData>>("TicketData") ?? new List<TicketData>();
            SelectedSeats = Request.Form["SelectedSeats"].ToString().Split(',').ToList();
            PassengerNames = existingTickets.Select(ticket => ticket.CustomerName).ToList();

            if (PassengerNames.Count != SelectedSeats.Count)
            {
                return BadRequest("The number of selected seats does not match the number of passengers.");
            }

            for (int i = 0; i < SelectedSeats.Count; i++)
            {
                var seat = SelectedSeats[i];
                var ticketData = new TicketData
                {
                    SeatNumber = seat.Substring(1),
                    CustomerName = PassengerNames[i],
                    TicketType = ticketType,
                    ClassType = seat.StartsWith("V") ? _classTypesConfig.Business : _classTypesConfig.Economy,
                    Carryluggage = existingTickets[i].Carryluggage,
                    Baggage = existingTickets[i].Baggage
                };

                existingTickets.Add(ticketData);
            }

            HttpContext.Session.SetObjectAsJson("TicketData", existingTickets);

            if (bookingId > 0)
            {
                return RedirectToPage("/Payment", new { id = bookingId });
            }

            return Redirect("/create-booking");
        }
    }
}
