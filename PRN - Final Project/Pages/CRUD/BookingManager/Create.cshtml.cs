using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using BussinessObjects;
using DataAccessObjects;

namespace PRN___Final_Project.Pages.CRUD.BookingManager
{
    public class CreateModel : PageModel
    {
        private readonly DataAccessObjects.Fall2024DbContext _context;

        public CreateModel(DataAccessObjects.Fall2024DbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["FlightId"] = new SelectList(_context.Flights, "FlightId", "FlightNumber");
        ViewData["ReturnFlightId"] = new SelectList(_context.Flights, "FlightId", "FlightNumber");
        ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return Page();
        }

        [BindProperty]
        public Booking Booking { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Bookings.Add(Booking);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
