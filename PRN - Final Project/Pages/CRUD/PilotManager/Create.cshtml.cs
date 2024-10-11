using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BussinessObjects;
using Services.Interfaces;

namespace PRN___Final_Project.Pages.CRUD.PilotPages
{
    public class CreateModel : PageModel
    {
        private readonly IPilotService _pilotService;

        public CreateModel(IPilotService pilotService)
        {
            _pilotService = pilotService;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Pilot Pilot { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

           await _pilotService.AddPilotAsync(Pilot);

            return RedirectToPage("./Index");
        }
    }
}
