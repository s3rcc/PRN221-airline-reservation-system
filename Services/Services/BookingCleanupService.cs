using BussinessObjects;
using Microsoft.Build.Framework;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class BookingCleanupService : BackgroundService
    {
        private readonly IBookingService _bookingService;
        private readonly IFlightService _flightService;
        private readonly ILogger<BookingCleanupService> _logger;
        private readonly ITicketService _ticketService;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1);

        public BookingCleanupService(IBookingService bookingService, IFlightService flightService, ILogger<BookingCleanupService> logger, ITicketService ticketService)
        {
            _bookingService = bookingService;
            _flightService = flightService;
            _logger = logger;
            _ticketService = ticketService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Booking Cleanup Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CleanupUnpaidBookingsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during booking cleanup.");
                }

                await Task.Delay(_checkInterval, stoppingToken); // Wait for the next execution
            }

            _logger.LogInformation("Booking Cleanup Service is stopping.");
        }

        private async Task CleanupUnpaidBookingsAsync()
        {
            var unpaidBooking = await _bookingService.GetAllBookingsAsync();

            foreach (var booking in unpaidBooking)
            {
                var flight = await _flightService.GetFlightByIdAsync(booking.FlightId);

                if (flight.DepartureDateTime < DateTime.Now && booking.PaymentStatus != "Paid")
                {
                    await _bookingService.DeleteBookingAsync(booking.BookingId);
                    _logger.LogInformation($"Deleted unpaid booking with ID: {booking.BookingId} (flight already departed)");
                }
            }
        }

        /// Check if the flight is fully booked
        //private async bool IsFlightFullyBooked(Flight flight)
        //{
        //    int totalSeats = flight.Plane.VipSeatNumber + flight.Plane.NormalSeatNumber;
        //    var tickets = await _ticketService.GetTicketsByFlightIdAsync(flight.FlightId);
        //    var bookings = await _bookingService.GetBookings();
        //    var total = tickets.Sum(t => t.)

        //    int bookedSeats = flight..Sum(ticket => ticket.AdultNum + ticket.ChildNum + ticket.BabyNum);

        //    return bookedSeats >= totalSeats;
        //}
    }
}
