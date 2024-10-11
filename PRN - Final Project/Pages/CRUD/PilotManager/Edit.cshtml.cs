using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BussinessObjects;
using Services.Interfaces;

namespace PRN___Final_Project.Pages.CRUD.PilotPages
{
    public class EditModel : PageModel
    {
        private readonly IPilotService _pilotService;

        public EditModel(IPilotService pilotService)
        {
            _pilotService = pilotService;
        }

        [BindProperty]
        public Pilot Pilot { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var pilot = await _pilotService.GetPilotByIdAsync(id);
            if (pilot == null)
            {
                return NotFound();
            }
            Pilot = pilot;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _pilotService.UpdatePilotAsync(Pilot);

            return RedirectToPage("./Index");
        }
    }
}
