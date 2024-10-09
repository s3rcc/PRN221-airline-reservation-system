using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BussinessObjects;
using DataAccessObjects;
using Services.Interfaces;
using Services.Services;

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
        public Pilot Pilot { get; set; }

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
