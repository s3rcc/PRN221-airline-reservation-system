using BussinessObjects;
using Microsoft.Build.Framework;
using Microsoft.Extensions.DependencyInjection;
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
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<BookingCleanupService> _logger;
        private readonly TimeSpan _outOfSeatCheckInterval = TimeSpan.FromMinutes(5);
        private readonly TimeSpan _unpaidBookingCheckInterval = TimeSpan.FromDays(1);

        public BookingCleanupService(IServiceProvider serviceProvider,ILogger<BookingCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Booking Cleanup Service is starting.");

            // Run both tasks concurrently
            var unpaidBookingsTask = RunUnpaidBookingCleanupAsync(stoppingToken);
            var outOfSeatBookingsTask = RunOutOfSeatCleanupAsync(stoppingToken);

            await Task.WhenAll(unpaidBookingsTask, outOfSeatBookingsTask);
        }

        private async Task RunUnpaidBookingCleanupAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Starting unpaid booking cleanup...");
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
                        var flightService = scope.ServiceProvider.GetRequiredService<IFlightService>();

                        await CleanupUnpaidBookingsAsync(bookingService, flightService);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during unpaid booking cleanup.");
                }

                // Wait for 1 day before running again
                await Task.Delay(_unpaidBookingCheckInterval, stoppingToken);
            }
        }

        private async Task RunOutOfSeatCleanupAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Starting out-of-seat booking cleanup...");
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
                        var flightService = scope.ServiceProvider.GetRequiredService<IFlightService>();

                        await CleanupUnpaidBookingsOutOfSeatAsync(bookingService, flightService);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during out-of-seat booking cleanup.");
                }

                // Wait for 5 minutes before running again
                await Task.Delay(_outOfSeatCheckInterval, stoppingToken);
            }
        }
        private async Task CleanupUnpaidBookingsAsync(IBookingService _bookingService, IFlightService _flightService)
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

        private async Task CleanupUnpaidBookingsOutOfSeatAsync(IBookingService _bookingService, IFlightService _flightService)
        {
            var allFlights = await _flightService.GetAllFlightsAsync();

            foreach (var flight in allFlights)
            {
                int totalSeats = flight.Plane.VipSeatNumber + flight.Plane.NormalSeatNumber;

                var bookingsFlight = await _bookingService.GetBookingByFlightIdAsync(flight.FlightId);

                int totalBookedSeats = bookingsFlight.Sum(b => b.AdultNum + b.ChildNum + b.BabyNum);

                // Check if the flight is overbooked or if there are unpaid bookings
                if (totalBookedSeats >= totalSeats)
                {
                    var unpaidBookings = bookingsFlight.Where(b => b.PaymentStatus != "Paid");

                    foreach (var booking in unpaidBookings)
                    {
                        // Cancel the unpaid booking
                        await _bookingService.DeleteBookingAsync(booking.BookingId);
                    }
                }
            }
        }
    }
}
