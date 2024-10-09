using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BussinessObjects;
using Services.Interfaces;

namespace PRN___Final_Project.Pages.CRUD.LocationPages
{
    public class DetailsModel : PageModel
    {
        private readonly ILocationService _locationService;

        public DetailsModel(ILocationService locationService)
        {
            _locationService = locationService;
        }

        public Location Location { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            Location = await _locationService.GetLocationByIdAsync(id);
            return Page();
        }
    }
}
