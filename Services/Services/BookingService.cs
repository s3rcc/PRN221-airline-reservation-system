using BussinessObjects;
using BussinessObjects.Exceptions;
using Repositories.Interface;
using Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Services.Services
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        public BookingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CancelBookingAsync(int id)
        {
            try
            {
                var booking = await _unitOfWork.Repository<Booking>().GetByIdAsync(id);
                if (booking == null)
                {
                    throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, "Booking not found.");
                }


                booking.Status = false;
                await _unitOfWork.Repository<Booking>().UpdateAsync(booking);
                await _unitOfWork.SaveChangeAsync();
            }
            catch
            {
                throw new ErrorException(StatusCodes.Status500InternalServerError, ErrorCode.INTERNAL_SERVER_ERROR, "Error canceling booking.");
            }
        }

        public async Task<int> CreateBookingAsync(Booking booking)
        {
            try
            {
                await _unitOfWork.Repository<Booking>().AddAsync(booking);
                await _unitOfWork.SaveChangeAsync();
                return booking.BookingId;
            }
            catch
            {
                throw new ErrorException(StatusCodes.Status500InternalServerError, ErrorCode.INTERNAL_SERVER_ERROR, "Error creating booking.");
            }
        }

        public async Task DeleteBookingAsync(int id)
        {
            try
            {
                var booking = await _unitOfWork.Repository<Booking>().GetByIdAsync(id) ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, "Booking not found.");
                _unitOfWork.Repository<Booking>().DeleteAsync(booking);
                await _unitOfWork.SaveChangeAsync();
            }
            catch
            {
                throw new ErrorException(StatusCodes.Status500InternalServerError, ErrorCode.INTERNAL_SERVER_ERROR, "Error deleting booking.");
            }
        }

        public async Task<IEnumerable<Booking>> GetAllBookingsAsync()
        {
            var bookings = await _unitOfWork.Repository<Booking>().GetAllAsync(
                includes: new Expression<Func<Booking, object>>[]
                {
            x => x.User,              
            x => x.Flight,
            x => x.ReturnFlight       
                });

            foreach (var booking in bookings)
            {
                if (booking.ReturnFlight == null)
                {
                    booking.ReturnFlight = new Flight();  
                    booking.ReturnFlight.FlightNumber = null;  
                }
            }

            return bookings;
        }


        public async Task<IEnumerable<Booking>> GetBookingByFlightIdAsync(int flightId)
        {
            var booking = await _unitOfWork.Repository<Booking>().FindAsync(x => x.FlightId == flightId && x.Status == true);
            return booking;
        }

        public async Task<Booking> GetBookingByIdAsync(int id)
        {
            var booking = await _unitOfWork.Repository<Booking>().GetByIdAsync(id,
                includes:
                    [
                    booking => booking.Flight,
                booking => booking.Flight.Destination,
                booking => booking.Flight.Origin,
                    ]) ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, "");
            return booking;
        }

        public async Task<IEnumerable<Booking>> GetBookingByUserIdAsync(string userId)
        {
            var bookings = await _unitOfWork.Repository<Booking>().FindAsync(
                x => x.UserId.Equals(userId),
                includes: new Expression<Func<Booking, object>>[]
                {
            b => b.Tickets,
            b => b.Flight.Destination,
            b => b.Flight.Origin,
            b => b.Flight,
            b => b.ReturnFlight  
                });

            foreach (var booking in bookings)
            {
                if (booking.ReturnFlight != null)
                {
                    var flightNumber = booking.ReturnFlight.FlightNumber;
                }
                else
                {
                    booking.ReturnFlight = new Flight();
                    booking.ReturnFlight.FlightNumber = null; // Gán FlightNumber là null
                }
            }

            return bookings;
        }



        public async Task<IEnumerable<Booking>> GetBookings(DateTime startDate, DateTime endDate)
        {
            try
            {
                return await _unitOfWork.Repository<Booking>().FindAsync(x => x.BookingDate >= startDate.Date && x.BookingDate <= endDate);
            }
            catch
            {
                throw new ErrorException(StatusCodes.Status500InternalServerError, ErrorCode.INTERNAL_SERVER_ERROR, "Error updating booking.");
            }
        }

        public async Task<IEnumerable<Booking>> GetBookingsByYear(int year)
        {
            try
            {
                return await _unitOfWork.Repository<Booking>().FindAsync(x => x.BookingDate.Year == year);
            }
            catch
            {
                throw new ErrorException(StatusCodes.Status500InternalServerError, ErrorCode.INTERNAL_SERVER_ERROR, "Error updating booking.");
            }
        }

		public async Task<int> GetTotalBooking()
		{
			try
			{
				var booking = await _unitOfWork.Repository<Booking>().GetAllAsync();
				return booking.Count();
			}
			catch
			{
				throw new ErrorException(StatusCodes.Status500InternalServerError, ErrorCode.INTERNAL_SERVER_ERROR, "Error getting total bookings");
			}
		}

		public async Task UpdateBookingAsync(Booking booking)
        {
            try
            {
                var existingBooking = await _unitOfWork.Repository<Booking>().GetByIdAsync(booking.BookingId) ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, "Booking not found.");
                await _unitOfWork.Repository<Booking>().UpdateAsync(booking);
                await _unitOfWork.SaveChangeAsync();
            }
            catch
            {
                throw new ErrorException(StatusCodes.Status500InternalServerError, ErrorCode.INTERNAL_SERVER_ERROR, "Error updating booking.");
            }
        }
    }
}
