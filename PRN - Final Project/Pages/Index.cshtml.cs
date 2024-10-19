using BussinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interfaces;

namespace PRN___Final_Project.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ILocationService _locationService;

        public IndexModel(ILogger<IndexModel> logger, ILocationService locationService)
        {
            _logger = logger;
            _locationService = locationService;
        }

        public IEnumerable<Location> Locations { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Locations = await _locationService.GetAllLocationsAsync();
        }
    }
}
