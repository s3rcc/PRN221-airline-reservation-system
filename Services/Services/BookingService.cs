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

                booking.Status = "Cancelled";
                await _unitOfWork.Repository<Booking>().UpdateAsync(booking);
                await _unitOfWork.SaveChangeAsync();
            }
            catch
            {
                throw new ErrorException(StatusCodes.Status500InternalServerError, ErrorCode.INTERNAL_SERVER_ERROR, "Error canceling booking.");
            }
        }

        public async Task CreateBookingAsync(Booking booking)
        {
            try
            {
                await _unitOfWork.Repository<Booking>().AddAsync(booking);
                await _unitOfWork.SaveChangeAsync();
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
            return await _unitOfWork.Repository<Booking>().GetAllAsync();
        }

        public async Task<Booking> GetBookingByIdAsync(int id)
        {
            var booking = await _unitOfWork.Repository<Booking>().GetByIdAsync(id) ?? throw new ErrorException(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, "");
            return booking;
        }

        public async Task<IEnumerable<Booking>> GetBookingByUserIdAsync(string userId)
        {
            return await _unitOfWork.Repository<Booking>().FindAsync(x => x.UserId.Equals(userId));
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
