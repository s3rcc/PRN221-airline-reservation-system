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
                bool shouldDeleteBooking = false;
                // Remove unpaid booking if the flight is set
                if (flight.DepartureDateTime < DateTime.Now && booking.PaymentStatus != "Paid")
                {
                    shouldDeleteBooking = true;
                    _logger.LogInformation($"Deleted unpaid booking with ID: {booking.BookingId} (flight already departed)");
                }

                // Remove unpaid booking if the seat is not available
                int requiredSeats = booking.AdultNum + booking.ChildNum + booking.BabyNum;
                if (IsSeatUnavailable(flight, booking.ClassType, requiredSeats))
                {
                    shouldDeleteBooking = true;
                    _logger.LogInformation($"Booking ID: {booking.BookingId} will be deleted (not enough seats for flight).");
                }

                // Condition 3: Check return flight if it exists
                if (booking.ReturnFlightId != null)
                {
                    var returnFlight = await _flightService.GetReturnFlightByIdAsync(booking.ReturnFlightId.Value);
                    if (IsSeatUnavailable(returnFlight, booking.ReturnClassType, requiredSeats))
                    {
                        shouldDeleteBooking = true;
                        _logger.LogInformation($"Booking ID: {booking.BookingId} will be deleted (not enough seats for return flight).");
                    }
                }

                // If any of the conditions are met, delete the booking
                if (shouldDeleteBooking)
                {
                    await _bookingService.DeleteBookingAsync(booking.BookingId);
                    _logger.LogInformation($"Deleted unpaid booking with ID: {booking.BookingId}");
                }
            }
        }

        private bool IsSeatUnavailable(Flight flight, string classType, int requiredSeats)
        {
            return (classType == "Normal" && flight.AvailableNormalSeat < requiredSeats) ||
                   (classType == "Vip" && flight.AvailableVipSeat < requiredSeats);
        }
    }
}
