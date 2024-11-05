using BussinessObjects;
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

        public async Task OnGetAsync(int? bookingId , bool? isOutbound, int? ticketId)
        {
            BookingId = bookingId.Value;
            
            if (!ticketId.HasValue && isOutbound.HasValue && bookingId.HasValue)
            {
                Tickets = await _ticketService.GetTicketByBookingIdAndTypeAsync(bookingId.Value, isOutbound);
            }
            else if (!isOutbound.HasValue && !ticketId.HasValue && bookingId.HasValue)
            {
                Tickets = await _ticketService.GetTicketsByBookingIdAsync(bookingId.Value);
            }
            else
            {
                var ticket = await _ticketService.GetTicketByIdAsync(ticketId.Value);
                Tickets = ticket != null ? new List<Ticket> { ticket } : Enumerable.Empty<Ticket>();
            }
        }
    }
}
