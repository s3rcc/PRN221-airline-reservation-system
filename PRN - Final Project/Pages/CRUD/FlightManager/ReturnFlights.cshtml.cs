using BussinessObjects;
using BussinessObjects.Config;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Services.Interfaces;
using Services.Services;

namespace PRN___Final_Project.Pages.CRUD.FlightManager
{
    public class ReturnFlightsModel : PageModel
    {
        private readonly IFlightService _flightService;
        private readonly ILocationService _locationService;
        private readonly UserManager<User> _userManager;
        private readonly ClassTypesConfig _classTypesConfig;
        private readonly TicketTypesConfig _ticketTypesConfig;
        private readonly PaymentStatusConfig _paymentStatusConfig;

        public ClassTypesConfig ClassTypesConfig => _classTypesConfig;
        public TicketTypesConfig TicketTypesConfig => _ticketTypesConfig;

        public ReturnFlightsModel(IFlightService flightService, ILocationService locationService, IOptions<ClassTypesConfig> classTypesConfig, IOptions<TicketTypesConfig> ticketTypesConfig, UserManager<User> userManager, IOptions<PaymentStatusConfig> paymentStatusConfig)
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
        public DateTime ReturnDate { get; set; }
        public int TotalPassengers { get; set; }
        public int AdultNum { get; set; }
        public int ChildNum { get; set; }
        public int BabyNum { get; set; }
        public string OriginLocation { get; set; }
        public string DestinationLocation { get; set; }
        public bool IsOneWay {  get; set; }
        public decimal TotalPrice { get; set; }
        public decimal ReturnTotalPrice { get; set; } = 0;

        public async Task OnGetAsync()
        {
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
                flightData.TotalPrice -= flightData.ReturnTotalPrice;
                TotalPrice = flightData.TotalPrice;
            }

            HttpContext.Session.SetObjectAsJson("FlightData", flightData);
            Flights = await _flightService.FilterFlightsAsync(DestinationId, OriginId, ReturnDate, TotalPassengers);
            var originLocation = await _locationService.GetLocationByIdAsync(DestinationId);
            OriginLocation = originLocation.LocationName;
            var destinationLocation = await _locationService.GetLocationByIdAsync(OriginId);
            DestinationLocation = destinationLocation.LocationName;
        }

        public async Task<IActionResult> OnPostAsync(decimal selectedPrice, int flightId, string selectedClass)
        {
            var flightData = HttpContext.Session.GetObjectFromJson<FlightData>("FlightData");
            var user = await _userManager.GetUserAsync(User);

            if (flightData != null)
            {
                flightData.TotalPrice += (selectedPrice * (flightData.AdultNum + flightData.ChildNum * 0.9m + flightData.BabyNum * 0.1m));
                flightData.ReturnTotalPrice = selectedPrice * (flightData.AdultNum + flightData.ChildNum * 0.9m + flightData.BabyNum * 0.1m);
                flightData.ReturnFlightId = flightId;
                flightData.ReturnClassType = selectedClass;
            }

            HttpContext.Session.SetObjectAsJson("FlightData", flightData);

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
    }
}
