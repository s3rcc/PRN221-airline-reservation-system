using BussinessObjects;
using Microsoft.AspNetCore.Identity;
using BussinessObjects.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Services.Interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Text;
using System.Text.Encodings.Web;

namespace PRN___Final_Project.Pages
{
    public class CheckInModel : PageModel
    {
        private readonly IAirPlaneService _airplaneService;
        private readonly ITicketService _ticketService;
        private readonly UserManager<User> _userManager;
        private readonly ClassTypesConfig _classTypesConfig;
        private readonly TicketTypesConfig _ticketTypesConfig;
        private readonly IEmailSender _emailSender;
        private readonly IBookingService _bookingService;

        public ClassTypesConfig ClassTypesConfig => _classTypesConfig;
        public TicketTypesConfig TicketTypesConfig => _ticketTypesConfig;

        public CheckInModel(IAirPlaneService airplaneService, ITicketService ticketService, IOptions<ClassTypesConfig> classTypesConfig, IOptions<TicketTypesConfig> ticketTypesConfig, UserManager<User> userManager, IEmailSender emailSender, IBookingService bookingService)
        {
            _airplaneService = airplaneService;
            _ticketService = ticketService;
            _classTypesConfig = classTypesConfig.Value;
            _ticketTypesConfig = ticketTypesConfig.Value;
            _userManager = userManager;
            _emailSender = emailSender;
            _bookingService = bookingService;
        }

        [BindProperty]
        public AirPlane AirPlane { get; set; }

        public int TotalRows { get; set; }
        public int TotalSeats { get; set; }
        public int AllowedSeats { get; set; }
        public string AllowedClassType { get; set; }

        [BindProperty]
        public List<string> SelectedSeats { get; set; }
        [BindProperty]
        public List<string> BookedSeats { get; set; }
        [BindProperty]
        public List<string> CustomerNames { get; set; }

        public async Task<IActionResult> OnGetAsync(int planeId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || !await _userManager.IsInRoleAsync(user, "Member"))
            {
                return RedirectToPage("/Errors/404");
            }

            AirPlane = await _airplaneService.GetAirPlaneByIdAsync(planeId);
            TotalSeats = AirPlane.VipSeatNumber + AirPlane.NormalSeatNumber;
            TotalRows = (int)Math.Ceiling(TotalSeats / 6.0);
            var flightType = HttpContext.Session.GetString("FlightType");
            var bookingData = HttpContext.Session.GetObjectFromJson<Booking>("BookingData");
            AllowedSeats = bookingData.AdultNum + bookingData.ChildNum + bookingData.BabyNum;
            
            if (flightType == _ticketTypesConfig.OutBoundFlight)
            {
                AllowedClassType = bookingData.ClassType;
            }
            else
            {
                AllowedClassType = bookingData.ReturnClassType;
            }

            SelectedSeats = new List<string>();
            BookedSeats = await _ticketService.GetBookedSeatsByFlightIdAsync(bookingData.FlightId, flightType);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(decimal carryLuggage, decimal baggage, List<string> customerNames)
        {
            var bookingData = HttpContext.Session.GetObjectFromJson<Booking>("BookingData");
            var booking = await _bookingService.GetBookingByIdAsync(bookingData.BookingId);
            var ticketType = HttpContext.Session.GetString("FlightType");
            var user = await _userManager.GetUserAsync(User);

            // Split the SelectedSeats string into a list
            SelectedSeats = Request.Form["SelectedSeats"].ToString().Split(',').ToList();

            var tickets = new List<Ticket>();

            for (int i = 0; i < SelectedSeats.Count; i++)
            {
                var seat = SelectedSeats[i];

                // Determine ClassType based on the first character of the seat number
                var classType = seat.StartsWith("V") ? _classTypesConfig.Business : _classTypesConfig.Economy;

                // Remove the first character to get the actual seat number
                var actualSeatNumber = seat.Substring(1);

                var ticket = new Ticket
                {
                    SeatNumber = actualSeatNumber, // Save the actual seat number
                    CustomerName = customerNames[i], // Add customer name
                    TicketType = ticketType,
                    IssuedDate = DateTime.UtcNow,
                    ClassType = classType,
                    Carryluggage = carryLuggage,
                    Baggage = baggage,
                    BookingId = bookingData.BookingId
                };
                tickets.Add(ticket);

                await _ticketService.CreateTicketAsync(ticket);
            }

            var emailContent = GenerateEmailContent(carryLuggage, baggage, booking, tickets);

            await _emailSender.SendEmailAsync(user.Email, "Your Ticket Information", emailContent);

            if (ticketType == _ticketTypesConfig.OutBoundFlight)
            {
                return RedirectToPage("/TicketDetails", new { bookingId = bookingData.BookingId, isOutbound = true });
            }
            else
            {
                return RedirectToPage("/TicketDetails", new { bookingId = bookingData.BookingId, isOutbound = false });
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
