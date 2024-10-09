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
    public class IndexModel : PageModel
    {
        private readonly IPilotService _plotService;

        public IndexModel(IPilotService pilotService)
        {
            _plotService = pilotService;
        }

        public IEnumerable<Pilot> Pilots { get;set; }

        public async Task OnGetAsync()
        {
            Pilots = await _plotService.GetAllPilotsAsync();
        }
    }
}
