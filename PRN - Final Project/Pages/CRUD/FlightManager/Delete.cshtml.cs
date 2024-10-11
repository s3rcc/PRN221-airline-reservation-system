using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BussinessObjects;
using Services.Interfaces;

namespace PRN___Final_Project.Pages.CRUD.FlightManager
{
    public class DeleteModel : PageModel
    {
        private readonly IFlightService _flightService;

        public DeleteModel(IFlightService flightService)
        {
            _flightService = flightService;
        }

        [BindProperty]
        public Flight Flight { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Flight = await _flightService.GetFlightByIdAsync(id);
            if (Flight == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            await _flightService.DeleteFlightAsync(id);

            return RedirectToPage("./Index");
        }
    }
}
