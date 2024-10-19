using BussinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interfaces;

namespace PRN___Final_Project.Pages.CRUD.PilotManager
{
    public class PilotManagementModel : PageModel
    {
        private readonly IPilotService _pilotService;
        public PilotManagementModel(IPilotService pilotService)
        {
            _pilotService = pilotService;
            Pilots = new List<Pilot>();
            Pilot = new Pilot();

        }
        [BindProperty]
        public Pilot Pilot { get; set; }
        public IEnumerable<Pilot> Pilots { get; set; }
        public async Task OnGetAsync()
        {
            Pilots = await _pilotService.GetAllPilotsAsync();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            Console.WriteLine($"Pilot Name: {Pilot.PilotName}, Status: {Pilot.Status}");

            if (Pilot.PilotId == 0)
            {
                await _pilotService.AddPilotAsync(Pilot);
            }
            else
            {
                await _pilotService.UpdatePilotAsync(Pilot);
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _pilotService.DeletePilotAsync(id);
            return RedirectToPage();
        }
    }
}
