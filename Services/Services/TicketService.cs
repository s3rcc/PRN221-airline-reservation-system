using BussinessObjects;
using BussinessObjects.Exceptions;
using Microsoft.AspNetCore.Http;
using Repositories.Interface;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

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
            var booking = await _unitOfWork.Repository<Booking>().GetByIdAsync(ticket.BookingId);
            if (booking == null || booking.Status)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.CONFLICT, "Cannot create ticket without a completed booking.");
            }

            await _unitOfWork.Repository<Ticket>().AddAsync(ticket);
            await _unitOfWork.SaveChangeAsync();
        }

        public async Task DeleteTicketAsync(int id)
        {
            var ticket = await _unitOfWork.Repository<Ticket>().GetByIdAsync(id) ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, "Ticket not found.");
            _unitOfWork.Repository<Ticket>().DeleteAsync(ticket);
            await _unitOfWork.SaveChangeAsync();
        }

        public async Task<IEnumerable<Ticket>> GetAllTicketsAsync()
        {
            return await _unitOfWork.Repository<Ticket>().GetAllAsync();
        }

        public async Task<Ticket> GetTicketByIdAsync(int id)
        {
            var ticket = await _unitOfWork.Repository<Ticket>().GetByIdAsync(id) ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, "Ticket not found.");
            return ticket;
        }

        public async Task<IEnumerable<Ticket>> GetTicketsByBookingIdAsync(int bookingId)
        {
            return await _unitOfWork.Repository<Ticket>().FindAsync(t => t.BookingId == bookingId);
        }

        public async Task UpdateTicketAsync(Ticket ticket)
        {
            var existingTicket = await _unitOfWork.Repository<Ticket>().GetByIdAsync(ticket.TicketId) ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, "Ticket not found.");
            existingTicket.SeatNumber = ticket.SeatNumber;
            existingTicket.TicketType = ticket.TicketType;
            existingTicket.Carryluggage = ticket.Carryluggage;
            existingTicket.Baggage = ticket.Baggage;
            existingTicket.ClassType = ticket.ClassType;

            await _unitOfWork.Repository<Ticket>().UpdateAsync(existingTicket);
            await _unitOfWork.SaveChangeAsync();
        }
    }
}
