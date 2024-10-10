using Microsoft.AspNetCore.Mvc.RazorPages;
using BussinessObjects;
using Services.Interfaces;

namespace PRN___Final_Project.Pages.CRUD.FlightManager
{
    public class IndexModel : PageModel
    {
        private readonly IFlightService _flightService;

        public IndexModel(IFlightService flightService)
        {
            _flightService = flightService;
        }

        public IEnumerable<Flight> Flight { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Flight = await _flightService.GetAllFlightsAsync();
        }
    }
}
