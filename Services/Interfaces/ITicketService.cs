using BussinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ITicketService
    {
        Task CreateTicketAsync(Ticket ticket);
        Task UpdateTicketAsync(Ticket ticket);
        Task DeleteTicketAsync(int id);
        Task<IEnumerable<Ticket>> GetAllTicketsAsync();
        Task<Ticket> GetTicketByIdAsync(int id);
        Task<IEnumerable<Ticket>> GetTicketsByBookingIdAsync(int bookingId);
        Task<IEnumerable<Ticket>> GetTicketByBookingIdAndTypeAsync(int bookingId, bool? isOutbound);
        Task<List<string>> GetBookedSeatsByFlightIdAsync(int flightId, string flightType);
    }
}
