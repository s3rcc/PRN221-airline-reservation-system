using Microsoft.AspNetCore.Mvc.RazorPages;
using BussinessObjects;
using Services.Interfaces;

namespace PRN___Final_Project.Pages.CRUD.PilotPages
{
    public class IndexModel : PageModel
    {
        private readonly IPilotService _plotService;

        public IndexModel(IPilotService pilotService)
        {
            _plotService = pilotService;
        }

        public IEnumerable<Pilot> Pilots { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Pilots = await _plotService.GetAllPilotsAsync();
        }
    }
}
