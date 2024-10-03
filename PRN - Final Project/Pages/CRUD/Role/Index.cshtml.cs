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
    public class IndexModel : PageModel
    {
        private readonly DataAccessObjects.Fall2024DbContext _context;

        public IndexModel(DataAccessObjects.Fall2024DbContext context)
        {
            _context = context;
        }

        public IList<BussinessObjects.Role> Role { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Role = await _context.Roles.ToListAsync();
        }
    }
}
