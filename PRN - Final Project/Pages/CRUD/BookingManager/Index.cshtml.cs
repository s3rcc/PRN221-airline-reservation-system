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

namespace PRN___Final_Project.Pages.CRUD.BookingManager
{
    public class IndexModel : PageModel
    {
        private readonly IBookingService _service;

        public IndexModel(IBookingService service)
        {
            _service = service;
            Booking = new();
            Bookings = new List<Booking>();
        }

        [BindProperty]
        public Booking Booking { get; set; }

        public IEnumerable<Booking> Bookings { get; set; }   

        public async Task OnGetAsync()
        {
            Bookings = await _service.GetAllBookingsAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Booking.BookingId == 0)
            {
                await _service.CreateBookingAsync(Booking);
            }
            else
            {
                await _service.UpdateBookingAsync(Booking);
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _service.DeleteBookingAsync(id);
            return RedirectToPage();
        }
    }
}
