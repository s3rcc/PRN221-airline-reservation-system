using Microsoft.AspNetCore.Mvc.RazorPages;
using BussinessObjects;
using Services.Interfaces;

namespace PRN___Final_Project.Pages.CRUD.AirPlaneManager
{
    public class IndexModel : PageModel
    {
        private readonly IAirPlaneService _airPlaneService;

        public IndexModel(IAirPlaneService airPlaneService)
        {
            _airPlaneService = airPlaneService;
        }

        public IEnumerable<AirPlane> AirPlane { get;set; } = default!;

        public async Task OnGetAsync()
        {
            AirPlane = await _airPlaneService.GetAllAirPlanesAsync();
        }
    }
}
