using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BussinessObjects;
using Services.Interfaces;

namespace PRN___Final_Project.Pages.CRUD.FlightManager
{
    public class DetailsModel : PageModel
    {
        private readonly IFlightService _flightService;

        public DetailsModel(IFlightService flightService)
        {
            _flightService = flightService;
        }

        public Flight Flight { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Flight = await _flightService.GetFlightByIdAsync(id);
            return Page();
        }
    }
}
