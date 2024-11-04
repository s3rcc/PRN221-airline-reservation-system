using BussinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IBookingService
    {
        Task<IEnumerable<Booking>> GetAllBookingsAsync();
        Task<Booking> GetBookingByIdAsync(int id);
        Task<IEnumerable<Booking>> GetBookingByUserIdAsync(string userId);
        Task<int> CreateBookingAsync(Booking booking);
        Task UpdateBookingAsync(Booking booking);
        Task DeleteBookingAsync(int id);
        Task CancelBookingAsync(int id);
        Task<IEnumerable<Booking>> GetBookingsByYear(int year);
        Task<IEnumerable<Booking>> GetBookings(DateTime startDate, DateTime endDate);
        Task<int> GetTotalBooking();
        Task<IEnumerable<Booking>> GetBookingByFlightIdAsync(int flightId);
    }
}
