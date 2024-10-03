using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BussinessObjects;
using DataAccessObjects;

namespace PRN___Final_Project.Pages.CRUD.Role
{
    public class DetailsModel : PageModel
    {
        private readonly DataAccessObjects.Fall2024DbContext _context;

        public DetailsModel(DataAccessObjects.Fall2024DbContext context)
        {
            _context = context;
        }

        public BussinessObjects.Role Role { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _context.Roles.FirstOrDefaultAsync(m => m.RoleId == id);
            if (role == null)
            {
                return NotFound();
            }
            else
            {
                Role = role;
            }
            return Page();
        }
    }
}
