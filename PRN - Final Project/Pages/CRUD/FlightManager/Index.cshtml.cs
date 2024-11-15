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
        public int PageSize { get; set; } = 20;
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }

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

            Flight = new Flight();  
            Flights = new List<Flight>();
            Planes = new List<AirPlane>();
            Pilots = new List<Pilot>();
            Locations = new List<Location>();

            StatusMessage = Noti.GetMsg();
            IsSuccess = Noti.IsSuccess;

        }

        public async Task<IActionResult> OnGetAsync(int pageIndex = 1, int pageSize = 20)
        {
            if (!(User.IsInRole("Staff") || User.IsInRole("Admin")))
            {
                return RedirectToPage("/Errors/404");
            }
            Planes = await _planeService.GetAllAirPlanesAsync();
            Pilots = await _pilotService.GetAllAvailablePilotsAsync();
            Locations = await _locationService.GetAllLocationsAsync();
            //Flights = await _flightService.GetAllFLightWithRealTimeCondition();
            var allFlights = await _flightService.GetAllFLightWithRealTimeCondition();
            TotalPages = (int)Math.Ceiling(allFlights.Count() / (double)pageSize);
            CurrentPage = pageIndex;

            Flights = await _flightService.GetAllFlightsWithPagitationAsync(pageIndex, pageSize);
            return Page();
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
                rs = await _flightService.AddFlightAsync(flight);
                action = "Create";
            }
            else
            {
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
