using BussinessObjects;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interfaces;

namespace PRN___Final_Project.Pages.CRUD.FlightManager
{
    public class FilteredFlightsModel : PageModel
    {
        private readonly IFlightService _flightService;

        public FilteredFlightsModel(IFlightService flightService)
        {
            _flightService = flightService;
        }

        public IEnumerable<Flight> Flights { get; set; } = default!;

        public async Task OnGetAsync(int originId, int destinationId, DateTime departureDate, DateTime? returnDate = null)
        {
            Flights = await _flightService.FilterFlightsAsync(originId, destinationId, departureDate, returnDate);
        }
    }
}
