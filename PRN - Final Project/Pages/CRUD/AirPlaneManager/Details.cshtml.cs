using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BussinessObjects;
using Services.Interfaces;

namespace PRN___Final_Project.Pages.CRUD.AirPlaneManager
{
    public class DetailsModel : PageModel
    {
        private readonly IAirPlaneService _airPlaneService;

        public DetailsModel(IAirPlaneService airPlaneService)
        {
            _airPlaneService = airPlaneService;
        }

        public AirPlane AirPlane { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            AirPlane = await _airPlaneService.GetAirPlaneByIdAsync(id);
            return Page();
        }
    }
}
