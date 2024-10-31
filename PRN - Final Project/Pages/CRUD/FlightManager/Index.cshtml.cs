using BussinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interfaces;


namespace PRN___Final_Project.Pages.CRUD.FlightManager
{
    public class FlightManagerModel : PageModel
    {
        [BindProperty]
        public Flight newFlight { get; } = new();

        [TempData]
        public string ValidMsg { get; set; }
        [BindProperty]
        public Flight Flight { get; set; }
        public IEnumerable<Flight> Flights { get; set; }
        public IEnumerable<AirPlane> Planes { get; set; }
        public IEnumerable<Pilot> Pilots { get; set; }
        public IEnumerable<Location> Locations { get; set; }

        public string StatusMessage { get; set; } = string.Empty;
        public bool IsSuccess { get; set; } = true;

        private readonly IFlightService _flightService;
        private readonly IAirPlaneService _planeService;
        private readonly IPilotService _pilotService;
        private readonly ILocationService _locationService;

        // Only inject services, not model objects
        public FlightManagerModel(IFlightService flightService, IAirPlaneService planeService, IPilotService pilotService, ILocationService locationService)
        {
            _flightService = flightService;
            _planeService = planeService;
            _pilotService = pilotService;
            _locationService = locationService;

            Flight = new Flight();  // Initialize model objects here
            Flights = new List<Flight>();
            Planes = new List<AirPlane>();
            Pilots = new List<Pilot>();
            Locations = new List<Location>();

            StatusMessage = Noti.GetMsg();
            IsSuccess = Noti.IsSuccess;

        }

        public async Task OnGetAsync()
        {
            Planes = await _planeService.GetAllAirPlanesAsync();
            Pilots = await _pilotService.GetAllPilotsAsync();
            Locations = await _locationService.GetAllLocationsAsync();
            Flights = await _flightService.GetAllFlightsAsync();  // Fetch flights if needed
        }

        public async Task<IActionResult> OnPostAsync(Flight flight)
        {

            //if (!ModelState.IsValid)
            //{
            //    return Page();
            //}
            var rs = string.Empty;
            var action = string.Empty;

            if (flight.FlightId == 0)
            {
                await Console.Out.WriteLineAsync("\n\n\n<On Create New>\n\n\n");
                rs = await _flightService.AddFlightAsync(flight);
                action = "Create";
            }
            else
            {
                await Console.Out.WriteLineAsync("\n\n\n<On Update>\n\n\n");
                rs = await _flightService.UpdateFlightAsync(flight);
                action = "Update";
            }
            ValidMsg = rs;
            Noti.SetByResult(action, "flight", rs);

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var rs = await _flightService.DeleteFlightAsync(id);

            Noti.SetByResult("Delete", "flight", rs);
            return RedirectToPage();
        }
    }
}
