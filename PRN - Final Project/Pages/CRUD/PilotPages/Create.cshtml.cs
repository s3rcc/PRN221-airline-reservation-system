using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using BussinessObjects;
using DataAccessObjects;
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
        public Pilot Pilot { get; set; }

        // For more information, see https://aka.ms/RazorPagesCRUD.
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
