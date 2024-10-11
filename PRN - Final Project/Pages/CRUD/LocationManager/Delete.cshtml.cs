using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BussinessObjects;
using Services.Interfaces;

namespace PRN___Final_Project.Pages.CRUD.LocationPages
{
    public class DeleteModel : PageModel
    {
        private readonly ILocationService _locationService;

        public DeleteModel(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [BindProperty]
        public Location Location { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Location = await _locationService.GetLocationByIdAsync(id);
            if (Location == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            await _locationService.DeleteLocationAsync(id);
            return RedirectToPage("Index");
        }
    }
}
