using Microsoft.AspNetCore.Mvc.RazorPages;
using BussinessObjects;
using Services.Interfaces;

namespace PRN___Final_Project.Pages.CRUD.LocationPages
{
    public class IndexModel : PageModel
    {
        private readonly ILocationService _locationService;

        public IndexModel(ILocationService locationService)
        {
            _locationService = locationService;
        }

        public IEnumerable<Location> Locations { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Locations = await _locationService.GetAllLocationsAsync();
        }
    }
}
