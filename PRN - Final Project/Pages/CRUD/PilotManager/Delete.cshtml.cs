using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BussinessObjects;
using Services.Interfaces;

namespace PRN___Final_Project.Pages.CRUD.PilotPages
{
    public class DeleteModel : PageModel
    {
        private readonly IPilotService _pilotService;

        public DeleteModel(IPilotService pilotService)
        {
            _pilotService = pilotService;
        }

        [BindProperty]
        public Pilot Pilot { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Pilot = await _pilotService.GetPilotByIdAsync(id);
            if (Pilot == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            await _pilotService.DeletePilotAsync(id);
            return RedirectToPage("Index");
        }
    }
}
