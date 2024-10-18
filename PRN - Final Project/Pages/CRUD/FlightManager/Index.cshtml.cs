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

        [BindProperty]
        public Flight Flight { get; set; }
        public IEnumerable<Flight> Flights { get; set; }
        public IEnumerable<AirPlane> Planes { get; set; }
        public IEnumerable<Pilot> Pilots { get; set; }
        public IEnumerable<Location> Locations { get; set; }

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
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (flight.FlightId == 0)
            {
                await _flightService.AddFlightAsync(flight);
            }
            else
            {
                await _flightService.UpdateFlightAsync(flight);
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _flightService.DeleteFlightAsync(id);
            return RedirectToPage();
        }
    }
}
