using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using BussinessObjects;
using Services.Interfaces;

namespace PRN___Final_Project.Pages.CRUD.FlightManager
{
    public class CreateModel : PageModel
    {
        private readonly IFlightService _flightService;
        private readonly ILocationService _locationService;
        private readonly IPilotService _pilotService;
        private readonly IAirPlaneService _airPlaneService;

        public CreateModel(IFlightService flightService, ILocationService locationService, IPilotService plotService, IAirPlaneService airPlaneService)
        {
            _flightService = flightService;
            _locationService = locationService;
            _pilotService = plotService;
            _airPlaneService = airPlaneService;
        }

        public async Task<IActionResult> OnGet()
        {
            var locations = await _locationService.GetAllLocationsAsync();
            var availablePilots = await _pilotService.GetAllAvailablePilotsAsync();
            var airPlanes = await _airPlaneService.GetAllAirPlanesAsync();

            ViewData["DestinationID"] = new SelectList(locations, "LocationID", "LocationName");
            ViewData["OriginID"] = new SelectList(locations, "LocationID", "LocationName");
            ViewData["PilotId"] = new SelectList(availablePilots, "PilotId", "PilotName");
            ViewData["PlaneId"] = new SelectList(airPlanes, "PlaneId", "PlaneName");
            return Page();
        }

        [BindProperty]
        public Flight Flight { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _flightService.AddFlightAsync(Flight);

            return RedirectToPage("./Index");
        }
    }
}
