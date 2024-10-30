using BussinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interfaces;

namespace PRN___Final_Project.Pages.CRUD.FlightManager
{
    public class FilterModel : PageModel
    {
        private readonly IFlightService _flightService;
        private readonly ILocationService _locationService;

        public FilterModel(IFlightService flightService, ILocationService locationService)
        {
            _flightService = flightService;
            _locationService = locationService;
        }

        [BindProperty]
        public int OriginId { get; set; }

        [BindProperty]
        public int DestinationId { get; set; }

        [BindProperty]
        public DateTime DepartureDate { get; set; } = DateTime.Now;
        [BindProperty]
        public DateTime? ReturnDate { get; set; } = null;

        public IEnumerable<Flight> FilteredFlights { get; set; } = default!;

        public IEnumerable<Location> Locations { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Locations = await _locationService.GetAllLocationsAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                //FilteredFlights = await _flightService.FilterFlightsAsync(OriginId, DestinationId, DepartureDate);
            }

            Locations = await _locationService.GetAllLocationsAsync();
            return Page();
        }
    }
}
