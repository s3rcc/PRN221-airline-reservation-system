using BussinessObjects;
using BussinessObjects.Config;
using BussinessObjects.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Repositories.Interface;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class TicketService : ITicketService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly TicketTypesConfig _ticketTypesConfig;
        public TicketService(IUnitOfWork unitOfWork, IOptions<TicketTypesConfig> ticketTypesConfig)
        {
            _unitOfWork = unitOfWork;
            _ticketTypesConfig = ticketTypesConfig.Value;
        }
        public async Task CreateTicketAsync(Ticket ticket)
        {
            var booking = await _unitOfWork.Repository<Booking>().GetByIdAsync(ticket.BookingId);

            if (booking == null || !booking.Status)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.CONFLICT, "Cannot create ticket without a completed booking.");
            }

            var existingTicket = await _unitOfWork.Repository<Ticket>().FindAsync(t => t.BookingId == ticket.BookingId && t.TicketType == ticket.TicketType);

            //if (existingTicket.Count() > 0)
            //{
            //    throw new ErrorException(StatusCodes.Status400BadRequest, ErrorCode.CONFLICT, $"Cannot create another {ticket.TicketType} ticket for this booking.");
            //}

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
            return await _unitOfWork.Repository<Ticket>().GetAllAsync(orderBy: x => x.OrderByDescending(x => x.IssuedDate));
        }

        public async Task<Ticket> GetTicketByIdAsync(int id)
        {
            var ticket = await _unitOfWork.Repository<Ticket>().GetByIdAsync(id,
                includes:
                [
                    t => t.Booking.Flight,
                t => t.Booking.Flight.Origin,
                t => t.Booking.Flight.Destination,
                t => t.Booking.User
                ]) ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, "Ticket not found.");
            return ticket;
        }

        public async Task<IEnumerable<Ticket>> GetTicketsByBookingIdAsync(int bookingId)
        {
            return await _unitOfWork.Repository<Ticket>().FindAsync(t => t.BookingId == bookingId,
                    includes:
                [
                    t => t.Booking.Flight,
                t => t.Booking.Flight.Origin,
                t => t.Booking.Flight.Destination,
                t => t.Booking.User
                ]);
        }

        public async Task<IEnumerable<Ticket>> GetTicketByBookingIdAndTypeAsync(int bookingId, bool? isOutbound)
        {
            if (isOutbound.HasValue)
            {
                var ticketType = isOutbound.Value ? _ticketTypesConfig.OutBoundFlight : _ticketTypesConfig.ReturnFlight;
                return await _unitOfWork.Repository<Ticket>().FindAsync(t => t.BookingId == bookingId && t.TicketType == ticketType,
                    includes:
                [
                    t => t.Booking.Flight,
                t => t.Booking.Flight.Origin,
                t => t.Booking.Flight.Destination,
                t => t.Booking.User
                ]);
            }
            else
            {
                return await _unitOfWork.Repository<Ticket>().FindAsync(t => t.BookingId == bookingId,
                    includes:
                [
                    t => t.Booking.Flight,
                t => t.Booking.Flight.Origin,
                t => t.Booking.Flight.Destination,
                t => t.Booking.User
                ]);
            }
        }

        public async Task<List<string>> GetBookedSeatsByFlightIdAsync(int flightId, string flightType)
        {
            IEnumerable<Booking> bookings = new List<Booking>();
            if (flightType == _ticketTypesConfig.OutBoundFlight)
            {
                bookings = await _unitOfWork.Repository<Booking>().FindAsync(booking => booking.FlightId == flightId);
            }
            else
            {
                bookings = await _unitOfWork.Repository<Booking>().FindAsync(booking => booking.ReturnFlightId == flightId);
            }

            var bookedTickets = new List<Ticket>();
            foreach (var booking in bookings)
            {
                var tickets = await _unitOfWork.Repository<Ticket>().FindAsync(ticket => ticket.BookingId == booking.BookingId);
                bookedTickets.AddRange(tickets);
            }

            if (flightType == _ticketTypesConfig.OutBoundFlight)
            {
                return bookedTickets
                    .Where(ticket => ticket.TicketType == _ticketTypesConfig.OutBoundFlight)
                    .Select(ticket => ticket.SeatNumber).ToList();
            }

            else
            {
                return bookedTickets
                    .Where(ticket => ticket.TicketType == _ticketTypesConfig.ReturnFlight)
                    .Select(ticket => ticket.SeatNumber).ToList();
            }
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
