using BussinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interfaces;

namespace PRN___Final_Project.Pages
{
    public class StatisticsModel : PageModel
    {
        private readonly IFlightService _flightService;
        private readonly IBookingService _bookingService;
        private readonly IPaymentService _paymentService;
        private readonly IAirPlaneService _airplaneService;
        private readonly IUserService _userService;
        private readonly IPilotService _pilotService;

        public List<string> Labels { get; set; } = new List<string>();
        public List<int> Bookings { get; set; } = new List<int>();
        public List<decimal> Revenue { get; set; } = new List<decimal>();

        [BindProperty(SupportsGet = true)]
        public int? SelectedYear { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? StartDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? EndDate { get; set; }

        public decimal TotalRevenue { get; set; }
        public int TotalBookings { get; set; }
        public int TotalAirplanes { get; set; }
        public int TotalFlights { get; set; }
        public int TotalPilots { get; set; }
        public int TotalUsers { get; set; }

        public StatisticsModel(IFlightService flightService, IBookingService bookingService, IPaymentService paymentService, IAirPlaneService airPlaneService, IUserService userService, IPilotService pilotService)
        {
            _flightService = flightService;
            _bookingService = bookingService;
            _paymentService = paymentService;
            _airplaneService = airPlaneService;
            _userService = userService;
            _pilotService = pilotService;
        }

        
        public async Task OnGetAsync()
        {
            TotalRevenue = await _paymentService.GetRevenue();
            TotalBookings = await _bookingService.GetTotalBooking();
            TotalAirplanes = await _airplaneService.GetTotalAirplane();
            TotalFlights = await _flightService.GetTotalFlight();
            TotalPilots = await _pilotService.GetTotalPilot();
            TotalUsers = await _userService.GetTotalUser();

            // Chart
            IEnumerable<Booking> bookings;
            IEnumerable<Payment> payments;

            if (SelectedYear.HasValue)
            {
                bookings = await _bookingService.GetBookingsByYear(SelectedYear.Value);
                payments = await _paymentService.GetPayments(SelectedYear.Value);
            }
            else if (StartDate.HasValue && EndDate.HasValue)
            {
                bookings = await _bookingService.GetBookings(StartDate.Value, EndDate.Value);
                payments = await _paymentService.GetPayments(StartDate.Value, EndDate.Value);
            }
            else
            {
                bookings = Enumerable.Empty<Booking>();
                payments = Enumerable.Empty<Payment>();
            }

            // Populate chart data
            Labels = bookings.Select(b => b.BookingDate.ToShortDateString()).Distinct().ToList();
            Bookings = bookings.GroupBy(b => b.BookingDate.ToShortDateString()).Select(g => g.Count()).ToList();
            Revenue = payments.GroupBy(p => p.PaymentDate.ToShortDateString()).Select(g => g.Sum(p => p.Amount)).ToList();
        }
    }
}
