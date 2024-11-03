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
using static System.Formats.Asn1.AsnWriter;

namespace Services.Services
{
    public class BookingCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<BookingCleanupService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1);

        public BookingCleanupService(IServiceProvider serviceProvider,ILogger<BookingCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Booking Cleanup Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
                        var flightService = scope.ServiceProvider.GetRequiredService<IFlightService>();

                        await CleanupUnpaidBookingsAsync(bookingService, flightService);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during booking reminder processing.");
                }

                await Task.Delay(_checkInterval, stoppingToken); // Wait for the next execution
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
