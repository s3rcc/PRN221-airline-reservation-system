using BussinessObjects;
using Microsoft.AspNetCore.Identity.UI.Services;
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
    public class BookingReminderService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<BookingReminderService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(24);
        public BookingReminderService(IServiceProvider serviceProvider, ILogger<BookingReminderService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Booking Reminder Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await SendBookingRemindersAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during booking reminder processing.");
                }

                await Task.Delay(_checkInterval, stoppingToken); // Wait for the next execution
            }

            _logger.LogInformation("Booking Reminder Service is stopping.");
        }

        private async Task SendBookingRemindersAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
                var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();

                var unpaidBookings = await bookingService.GetAllBookingsAsync();

                foreach (var booking in unpaidBookings)
                {
                    // Check if the booking is unpaid and nearing the deadline
                    if (booking.PaymentStatus != "Paid")
                    {
                        var daysSinceBooking = (DateTime.Now - booking.BookingDate).Days;

                        if (daysSinceBooking == 7 || daysSinceBooking == 5 || daysSinceBooking == 3)
                        {
                            // Send reminder email
                            await SendReminderEmailAsync(booking, emailSender);
                        }
                    }
                }
            }
        }

        private async Task SendReminderEmailAsync(Booking booking, IEmailSender _emailSender)
        {
            var user = booking.User; // Assuming User is included in the Booking entity

            string subject = $"Payment Reminder for Your Booking {booking.BookingId}";
            string htmlMessage = $@"
            <p>Dear {user.UserName},</p>
            <p>This is a reminder to complete the payment for your booking with ID: {booking.BookingId}.</p>
            <p>You have {7 - (DateTime.Now - booking.BookingDate).Days} days left to complete your payment.</p>
            <p>Please visit our platform and finalize your booking.</p>
            <p>Thank you!</p>";

            try
            {
                await _emailSender.SendEmailAsync(user.Email, subject, htmlMessage);
                _logger.LogInformation($"Reminder email sent to {user.Email} for booking ID: {booking.BookingId}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending email to {user.Email}: {ex.Message}");
            }
        }
    }
}
