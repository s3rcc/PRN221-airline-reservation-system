using Microsoft.AspNetCore.Mvc.RazorPages;
using BussinessObjects;
using Services.Interfaces;

namespace PRN___Final_Project.Pages.CRUD.BookingManager
{
    public class IndexModel : PageModel
    {
        private readonly IBookingService _bookingService;

        public IndexModel(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        public IEnumerable<Booking> Booking { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Booking = await _bookingService.GetAllBookingsAsync();
        }
    }
}
