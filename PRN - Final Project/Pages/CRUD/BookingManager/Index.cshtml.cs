using Microsoft.AspNetCore.Mvc.RazorPages;
using BussinessObjects;
using DataAccessObjects;
using Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
        public int PageSize { get; set; } = 20;
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }

        public IEnumerable<Booking> Bookings { get; set; }   

        public async Task<IActionResult> OnGetAsync(int pageIndex = 1, int pageSize = 20)
        {
            if (!(User.IsInRole("Staff") || User.IsInRole("Admin")))
            {
                return RedirectToPage("/Errors/404");
            }
            //Bookings = await _service.GetAllBookingsAsync();
            var allBookings = await _service.GetAllBookingsAsync();
            TotalPages = (int)Math.Ceiling(allBookings.Count() / (double)pageSize);
            CurrentPage = pageIndex;

            Bookings = await _service.GetAllBookingsAsync(pageIndex, pageSize);
            return Page();
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
