using BussinessObjects;
using BussinessObjects.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interfaces;
using System.Text.Encodings.Web;
using System.Text;
using Microsoft.AspNetCore.Identity.UI.Services;
using DataAccessObjects.Migrations;

namespace PRN___Final_Project.Pages
{
    public class PaymentCallbackModel : PageModel
    {
        private readonly IPaymentService _paymentService;
        private readonly ITicketService _ticketService;
        private readonly IBookingService _bookingService;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<User> _userManager;

        public PaymentCallbackModel(IPaymentService paymentService, ITicketService ticketService, IBookingService bookingService, IEmailSender emailSender, UserManager<User> userManager)
        {
            _paymentService = paymentService;
            _ticketService = ticketService;
            _bookingService = bookingService;
            _emailSender = emailSender;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var bookingId = HttpContext.Session.GetInt32("BookingId");
                var user = await _userManager.GetUserAsync(User);
                List<TicketData> ticketDatas = HttpContext.Session.GetObjectFromJson<List<TicketData>>("TicketData");
                await _paymentService.ExecutePayment(Request.Query);
                var tickets = new List<Ticket>();
                if (ticketDatas != null && ticketDatas.Any())
                {
                    foreach (var ticketData in ticketDatas)
                    {
                        var ticket = new Ticket
                        {
                            BookingId = bookingId.Value > 0 ? bookingId.Value : ticketData.BookingId,
                            SeatNumber = ticketData.SeatNumber,
                            TicketType = ticketData.TicketType,
                            CustomerName = ticketData.CustomerName,
                            IssuedDate = DateTime.UtcNow,
                            Carryluggage = ticketData.Carryluggage,
                            Baggage = ticketData.Baggage,
                            ClassType = ticketData.ClassType
                            
                        };
                        tickets.Add(ticket);
                        await _ticketService.CreateTicketAsync(ticket);
                    }

                    var booking = new Booking();
                    if (bookingId > 0)
                    {
                        booking = _bookingService.GetBookingByIdAsync(bookingId.Value).Result;
                    }
                    else
                    {
                        booking = _bookingService.GetBookingByIdAsync(ticketDatas.Select(x => x.BookingId).FirstOrDefault()).Result;
                    }

                    var emailContent = GenerateEmailContent(ticketDatas.Select(x => x.Carryluggage).FirstOrDefault(), ticketDatas.Select(x => x.Baggage).FirstOrDefault(), booking, tickets);

                    await _emailSender.SendEmailAsync(user.Email, "Your Ticket Information", emailContent);

                    HttpContext.Session.Remove("TicketData");
                }

                if (TempData.ContainsKey("BookingId"))
                {
                    TempData.Remove("BookingId");
                }

                HttpContext.Session.Remove("BookingId");

                return RedirectToPage("/PaymentResult", new { status = "success" });
            }
            catch (ErrorException ex)
            {
                HttpContext.Session.Remove("BookingId");
                return RedirectToPage("/PaymentResult", new { status = "failed", message = ex.Message });
            }
        }

        private string GenerateEmailContent(decimal carryLuggage, decimal baggage, Booking booking, List<Ticket> tickets)
        {
            var callbackUrl = Url.Page(
        "/TicketDetails",
        pageHandler: null,
        values: new { bookingId = booking.BookingId },
        protocol: Request.Scheme);

            var sb = new StringBuilder();

            // General Information
            sb.AppendLine("<strong>General Information:</strong><br/>");
            sb.AppendLine($"Origin Location: {booking.Flight.Origin.LocationName}<br/>");
            sb.AppendLine($"Destination Location: {booking.Flight.Destination.LocationName}<br/>");
            sb.AppendLine($"Departure Date: {booking.Flight.DepartureDateTime}<br/>");
            sb.AppendLine($"Carry-on Luggage: {carryLuggage}<br/>");
            sb.AppendLine($"Checked Baggage: {baggage}<br/><br/>"); // Add a blank line for separation

            // Personal Information
            sb.AppendLine("<strong>Personal Information:</strong><br/>");
            foreach (var ticket in tickets)
            {
                sb.AppendLine($"Customer Name: {ticket.CustomerName}<br/>");
                sb.AppendLine($"Seat Number: {ticket.SeatNumber}<br/>");
                sb.AppendLine($"Ticket Type: {ticket.TicketType}<br/>");
                sb.AppendLine($"Class Type: {ticket.ClassType}<br/><br/>"); // Add a blank line after each ticket for better spacing
            }

            sb.AppendLine("To view your ticket(s), " +
                  $"<a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>click here</a>.");

            return sb.ToString();
        }
    }
}
