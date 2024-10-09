using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BussinessObjects;
using DataAccessObjects;

namespace PRN___Final_Project.Pages.CRUD.BookingManager
{
    public class IndexModel : PageModel
    {
        private readonly DataAccessObjects.Fall2024DbContext _context;

        public IndexModel(DataAccessObjects.Fall2024DbContext context)
        {
            _context = context;
        }

        public IList<Booking> Booking { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Booking = await _context.Bookings
                .Include(b => b.Flight)
                .Include(b => b.ReturnFlight)
                .Include(b => b.User).ToListAsync();
        }
    }
}
