using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BussinessObjects;
using Services.Interfaces;

namespace PRN___Final_Project.Pages.CRUD.AirPlaneManager
{
    public class CreateModel : PageModel
    {
        private readonly IAirPlaneService _airPlaneService;

        public CreateModel(IAirPlaneService airPlaneSevice)
        {
            _airPlaneService = airPlaneSevice;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public AirPlane AirPlane { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _airPlaneService.AddAirPlaneAsync(AirPlane);

            return RedirectToPage("./Index");
        }
    }
}
