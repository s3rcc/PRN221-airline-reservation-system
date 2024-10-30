using BussinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interfaces;

namespace PRN___Final_Project.Pages
{
    public class TicketDetailsModel : PageModel
    {
        private readonly ITicketService _ticketService;

        public TicketDetailsModel(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        public IEnumerable<Ticket> Tickets { get; set; }
        public int BookingId { get; set; }

        public async Task OnGetAsync(int bookingId, bool isOutbound)
        {
            BookingId = bookingId;
            Tickets = await _ticketService.GetTicketByBookingIdAndTypeAsync(bookingId, isOutbound);
        }
    }
}
