using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BussinessObjects;
using DataAccessObjects;
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
        public Pilot Pilot { get; set; }

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

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
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
