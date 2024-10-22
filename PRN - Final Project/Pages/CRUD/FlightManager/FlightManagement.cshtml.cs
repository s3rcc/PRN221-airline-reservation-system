using BussinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interfaces;

namespace PRN___Final_Project.Pages.CRUD.FlightManager
{
    public class FlightManagementModel : PageModel
    {
        private readonly IFlightService _flightService;

        public FlightManagementModel(IFlightService flightService)
        {
            _flightService = flightService;
            Flights = new List<Flight>();
            Flight = new Flight();
        }

        [BindProperty]
        public Flight Flight { get; set; }
        public IEnumerable<Flight> Flights { get; set; }

        public async Task OnGetAsync()
        {
            Flights = await _flightService.GetAllFlightsAsync();
        }
    }
}
