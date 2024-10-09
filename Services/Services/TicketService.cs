using BussinessObjects.Exceptions;
using BussinessObjects;
using Microsoft.AspNetCore.Http;
using Repositories.Interface;
using Services.Interfaces;

namespace Services.Services
{
    public class TicketService : ITicketService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TicketService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CreateTicketAsync(Ticket ticket)
        {
            try
            {
                await _unitOfWork.Repository<Ticket>().AddAsync(ticket);
                await _unitOfWork.SaveChangeAsync();
            }
            catch
            {
                throw new ErrorException(
                    StatusCodes.Status500InternalServerError,
                    ErrorCode.INTERNAL_SERVER_ERROR, 
                    "Error creating ticket.");
            }
        }

        public async Task DeleteTicketAsync(int id)
        {
            try
            {
                var ticket = await _unitOfWork.Repository<Ticket>().GetByIdAsync(id)
                    ?? throw 
                    new ErrorException(
                        StatusCodes.Status404NotFound, 
                        ErrorCode.NOT_FOUND, 
                        "Ticket not found.");

                _unitOfWork.Repository<Ticket>().DeleteAsync(ticket);
                await _unitOfWork.SaveChangeAsync();
            }
            catch
            {
                throw new ErrorException(
                    StatusCodes.Status500InternalServerError, 
                    ErrorCode.INTERNAL_SERVER_ERROR, 
                    "Error deleting ticket.");
            }
        }

        public async Task<IEnumerable<Ticket>> GetAllTicketsAsync()
        {
            return await _unitOfWork.Repository<Ticket>().GetAllAsync();
        }

        public async Task<Ticket> GetTicketByIdAsync(int id)
        {
            var ticket = await _unitOfWork.Repository<Ticket>().GetByIdAsync(id)
                ?? throw 
                new ErrorException(
                    StatusCodes.Status404NotFound, 
                    ErrorCode.NOT_FOUND, 
                    "Ticket not found.");
            return ticket;
        }

        public async Task<IEnumerable<Ticket>> GetTicketsByBookingIdAsync(int bookingId)
        {
            return await _unitOfWork.Repository<Ticket>().FindAsync(x => x.BookingId.Equals(bookingId));
        }

        public async Task UpdateTicketAsync(Ticket ticket)
        {
            try
            {
                var existingTicket = await _unitOfWork.Repository<Ticket>().GetByIdAsync(ticket.TicketId)
                    ?? throw 
                    new ErrorException(
                        StatusCodes.Status404NotFound, 
                        ErrorCode.NOT_FOUND, 
                        "Ticket not found.");
                await _unitOfWork.Repository<Ticket>().UpdateAsync(ticket);
                await _unitOfWork.SaveChangeAsync();
            }
            catch
            {
                throw new ErrorException(
                    StatusCodes.Status500InternalServerError, 
                    ErrorCode.INTERNAL_SERVER_ERROR, 
                    "Error updating ticket.");
            }
        }
    }
}
