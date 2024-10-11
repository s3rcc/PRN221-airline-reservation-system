using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BussinessObjects;
using Services.Interfaces;

namespace PRN___Final_Project.Pages.CRUD.AirPlaneManager
{
    public class DeleteModel : PageModel
    {
        private readonly IAirPlaneService _airPlaneService;

        public DeleteModel(IAirPlaneService airPlaneService)
        {
            _airPlaneService = airPlaneService;
        }

        [BindProperty]
        public AirPlane AirPlane { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            AirPlane = await _airPlaneService.GetAirPlaneByIdAsync(id);
            if (AirPlane == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            await _airPlaneService.DeleteAirPlaneAsync(id);
            return RedirectToPage("Index");
        }
    }
}
