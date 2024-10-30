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
