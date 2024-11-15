using BussinessObjects;
using BussinessObjects.Config;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Services.Interfaces;

namespace PRN___Final_Project.Pages.CRUD.FlightManager
{
    public class OutboundFlightsModel : PageModel
    {
        private readonly IFlightService _flightService;
        private readonly ILocationService _locationService;
        private readonly ClassTypesConfig _classTypesConfig;
        private readonly TicketTypesConfig _ticketTypesConfig;
        private readonly PaymentStatusConfig _paymentStatusConfig;
        private readonly UserManager<User> _userManager;

        public ClassTypesConfig ClassTypesConfig => _classTypesConfig;
        public TicketTypesConfig TicketTypesConfig => _ticketTypesConfig;

        public OutboundFlightsModel(IFlightService flightService, ILocationService locationService, IOptions<ClassTypesConfig> classTypesConfig, IOptions<TicketTypesConfig> ticketTypesConfig, UserManager<User> userManager, IOptions<PaymentStatusConfig> paymentStatusConfig)
        {
            _flightService = flightService;
            _locationService = locationService;
            _classTypesConfig = classTypesConfig.Value;
            _ticketTypesConfig = ticketTypesConfig.Value;
            _userManager = userManager;
            _paymentStatusConfig = paymentStatusConfig.Value;
        }

        public IEnumerable<Flight> Flights { get; set; } = default!;
        public int OriginId { get; set; }
        public int DestinationId { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public int TotalPassengers { get; set; }
        public int AdultNum { get; set; }
        public int ChildNum { get; set; }
        public int BabyNum { get; set; }
        public string OriginLocation { get; set; }
        public string DestinationLocation { get; set; }
        public bool IsOneWay { get; set; }
        public decimal OutboundTotalPrice { get; set; } = 0;
        public decimal TotalPrice { get; set; } = 0;

        public async Task<IActionResult> OnGetAsync()
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để tiếp tục.";
                return Redirect("/login");
            }

            var flightData = HttpContext.Session.GetObjectFromJson<FlightData>("FlightData");

            if (flightData != null)
            {
                OriginId = flightData.OriginId;
                DestinationId = flightData.DestinationId;
                DepartureDate = flightData.DepartureDate;
                ReturnDate = flightData.ReturnDate;
                TotalPassengers = flightData.TotalPassengers;
                IsOneWay = flightData.IsOneWay;
                AdultNum = flightData.AdultNum;
                ChildNum = flightData.ChildNum;
                BabyNum = flightData.BabyNum;
                flightData.TotalPrice = 0;
                flightData.ReturnTotalPrice = 0;
                flightData.OutboundTotalPrice = 0;
                TotalPrice = flightData.TotalPrice;
                OutboundTotalPrice = flightData.OutboundTotalPrice;
            }

            HttpContext.Session.SetObjectAsJson("FlightData", flightData);
            Flights = await _flightService.FilterFlightsAsync(OriginId, DestinationId, DepartureDate, TotalPassengers);
            var originLocation = await _locationService.GetLocationByIdAsync(OriginId);
            OriginLocation = originLocation.LocationName;
            var destinationLocation = await _locationService.GetLocationByIdAsync(DestinationId);
            DestinationLocation = destinationLocation.LocationName;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(decimal selectedPrice, int flightId, string selectedClass)
        {
            var flightData = HttpContext.Session.GetObjectFromJson<FlightData>("FlightData");
            var user = await _userManager.GetUserAsync(User);

            if (flightData != null)
            {
                flightData.OutboundTotalPrice = selectedPrice * (flightData.AdultNum + flightData.ChildNum * 0.9m + flightData.BabyNum * 0.1m);
                flightData.TotalPrice = flightData.OutboundTotalPrice;
                flightData.OutboundFlightId = flightId;
                flightData.ClassType = selectedClass;
            }

            HttpContext.Session.SetObjectAsJson("FlightData", flightData);

            if (flightData.IsOneWay)
            {
                var booking = new Booking();
                booking.UserId = user.Id;
                booking.FlightId = flightData.OutboundFlightId;
                booking.ReturnFlightId = flightData.ReturnFlightId;
                booking.TotalPrice = flightData.TotalPrice;
                booking.PaymentStatus = _paymentStatusConfig.Unpaid;
                booking.Status = true;
                booking.AdultNum = flightData.AdultNum;
                booking.ChildNum = flightData.ChildNum;
                booking.BabyNum = flightData.BabyNum;
                booking.ClassType = flightData.ClassType;
                booking.ReturnClassType = flightData.ReturnClassType;

                HttpContext.Session.SetObjectAsJson("BookingData", booking);

                return RedirectToPage("/CRUD/BookingManager/BookingDetail");
            }

            return RedirectToPage("./ReturnFlights");
        }
    }
}
