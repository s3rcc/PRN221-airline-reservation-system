using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using BussinessObjects;
using Services.Interfaces;

namespace PRN___Final_Project.Pages.CRUD.FlightManager
{
    public class EditModel : PageModel
    {
        private readonly IFlightService _flightService;
        private readonly ILocationService _locationService;
        private readonly IPilotService _pilotService;
        private readonly IAirPlaneService _airPlaneService;

        public EditModel(IFlightService flightService, ILocationService locationService, IPilotService pilotService, IAirPlaneService airPlaneService)
        {
            _flightService = flightService;
            _locationService = locationService;
            _pilotService = pilotService;
            _airPlaneService = airPlaneService;
        }

        [BindProperty]
        public Flight Flight { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var flight = await _flightService.GetFlightByIdAsync(id);

            if (flight == null)
            {
                return NotFound();
            }

            Flight = flight;

            var locations = await _locationService.GetAllLocationsAsync();
            var availablePilots = await _pilotService.GetAllAvailablePilotsAsync();
            var airPlanes = await _airPlaneService.GetAllAirPlanesAsync();

            ViewData["DestinationID"] = new SelectList(locations, "LocationID", "LocationName");
            ViewData["OriginID"] = new SelectList(locations, "LocationID", "LocationName");
            ViewData["PilotId"] = new SelectList(availablePilots, "PilotId", "PilotName");
            ViewData["PlaneId"] = new SelectList(airPlanes, "PlaneId", "PlaneName");
            return Page();
        }
        
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _flightService.UpdateFlightAsync(Flight);

            return RedirectToPage("./Index");
        }
    }
}
