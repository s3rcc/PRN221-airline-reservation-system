using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BussinessObjects;
using Services.Interfaces;

namespace PRN___Final_Project.Pages.CRUD.LocationPages
{
    public class EditModel : PageModel
    {
        private readonly ILocationService _locationService;

        public EditModel(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [BindProperty]
        public Location Location { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var location = await _locationService.GetLocationByIdAsync(id);
            if (location == null)
            {
                return NotFound();
            }
            Location = location;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _locationService.UpdateLocationAsync(Location);

            return RedirectToPage("./Index");
        }
    }
}
