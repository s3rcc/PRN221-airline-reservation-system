using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BussinessObjects;
using Services.Interfaces;

namespace PRN___Final_Project.Pages.CRUD.LocationPages
{
    public class CreateModel : PageModel
    {
        private readonly ILocationService _locationService;

        public CreateModel(ILocationService locationService)
        {
            _locationService = locationService;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Location Location { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _locationService.AddLocationAsync(Location);

            return RedirectToPage("./Index");
        }
    }
}
