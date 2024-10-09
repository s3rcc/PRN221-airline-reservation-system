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
using Services.Services;

namespace PRN___Final_Project.Pages.CRUD.LocationPages
{
    public class EditModel : PageModel
    {
        private readonly ILocationService _locationService;

        public EditModel(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [BindProperty]
        public Location Location { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var location = await _locationService.GetLocationByIdAsync(id);
            if (location == null)
            {
                return NotFound();
            }
            Location = location;
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

            await _locationService.UpdateLocationAsync(Location);

            return RedirectToPage("./Index");
        }
    }
}
