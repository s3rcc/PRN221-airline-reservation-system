using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BussinessObjects;
using Services.Interfaces;

namespace PRN___Final_Project.Pages.CRUD.AirPlaneManager
{
    public class EditModel : PageModel
    {
        private readonly IAirPlaneService _airPlaneService;

        public EditModel(IAirPlaneService airPlaneService)
        {
            _airPlaneService = airPlaneService;
        }

        [BindProperty]
        public AirPlane AirPlane { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var airPlane = await _airPlaneService.GetAirPlaneByIdAsync(id);
            if (airPlane == null)
            {
                return NotFound();
            }
            AirPlane = airPlane;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _airPlaneService.UpdateAirPlaneAsync(AirPlane);

            return RedirectToPage("./Index");
        }
    }
}
