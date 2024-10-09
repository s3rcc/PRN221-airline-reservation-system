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

namespace PRN___Final_Project.Pages.CRUD.PilotPages
{
    public class DetailsModel : PageModel
    {
        private readonly IPilotService _pilotService;

        public DetailsModel(IPilotService pilotService)
        {
            _pilotService = pilotService;
        }

        public Pilot Pilot { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {

            Pilot = await _pilotService.GetPilotByIdAsync(id);
            return Page();
        }
    }
}
