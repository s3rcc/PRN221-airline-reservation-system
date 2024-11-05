using BussinessObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interfaces; // Ensure to import your service interface

namespace PRN___Final_Project.Areas.Identity.Pages.Account.Manage
{
    [Authorize]
    public class ListTicketModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly IBookingService _bookingService;
        private readonly ITicketService _ticketService; // Inject ticket service

        public IEnumerable<Ticket> Tickets { get; set; } // Property to hold the tickets

        public ListTicketModel(UserManager<User> userManager, IBookingService bookingService, ITicketService ticketService)
        {
            _userManager = userManager;
            _bookingService = bookingService;
            _ticketService = ticketService; // Initialize ticket service
            Tickets = new List<Ticket>();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("Unable to load user.");
            }

            if (!await _userManager.IsInRoleAsync(user, "Member"))
            {
                return RedirectToPage("/Errors/404");
            }

            var bookings = await _bookingService.GetBookingByUserIdAsync(user.Id);

            var allTickets = new List<Ticket>();
            foreach (var booking in bookings)
            {
                var tickets = await _ticketService.GetTicketsByBookingIdAsync(booking.BookingId);
                allTickets.AddRange(tickets);
            }

            Tickets = allTickets;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int bookingId, int ticketId)
        {
            return RedirectToPage("/TicketDetails", new { bookingId , ticketId });
        }
    }
}
