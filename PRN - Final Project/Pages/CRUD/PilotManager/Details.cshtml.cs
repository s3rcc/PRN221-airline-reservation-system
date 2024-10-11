using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BussinessObjects;
using Services.Interfaces;

namespace PRN___Final_Project.Pages.CRUD.PilotPages
{
    public class DetailsModel : PageModel
    {
        private readonly IPilotService _pilotService;

        public DetailsModel(IPilotService pilotService)
        {
            _pilotService = pilotService;
        }

        public Pilot Pilot { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {

            Pilot = await _pilotService.GetPilotByIdAsync(id);
            return Page();
        }
    }
}
