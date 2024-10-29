using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interfaces;
using BussinessObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN___Final_Project.Pages.CRUD.TicketManager
{
    public class IndexModel : PageModel
    {
        private readonly ITicketService _ticketService;

        public IndexModel(ITicketService ticketService)
        {
            _ticketService = ticketService;
            Tickets = new List<Ticket>();
            Ticket = new Ticket();
        }

        [BindProperty]
        public Ticket Ticket { get; set; }

        public IEnumerable<Ticket> Tickets { get; set; }

        public async Task OnGetAsync()
        {
            Tickets = await _ticketService.GetAllTicketsAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Ticket.TicketId == 0)
            {
                await _ticketService.CreateTicketAsync(Ticket);
            }
            else
            {
                await _ticketService.UpdateTicketAsync(Ticket);
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _ticketService.DeleteTicketAsync(id);
            return RedirectToPage();
        }
    }
}
