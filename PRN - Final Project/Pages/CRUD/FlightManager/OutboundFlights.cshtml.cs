using BussinessObjects;
using BussinessObjects.Config;
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

        public ClassTypesConfig ClassTypesConfig => _classTypesConfig;
        public TicketTypesConfig TicketTypesConfig => _ticketTypesConfig;

        public OutboundFlightsModel(IFlightService flightService, ILocationService locationService, IOptions<ClassTypesConfig> classTypesConfig, IOptions<TicketTypesConfig> ticketTypesConfig)
        {
            _flightService = flightService;
            _locationService = locationService;
            _classTypesConfig = classTypesConfig.Value;
            _ticketTypesConfig = ticketTypesConfig.Value;
        }

        public IEnumerable<Flight> Flights { get; set; } = default!;
        public int OriginId { get; set; }
        public int DestinationId { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public int TotalPassengers { get; set; }
        public string OriginLocation { get; set; }
        public string DestinationLocation { get; set; }
        public bool IsOneWay { get; set; }
        public string ClassType { get; set; }

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
            }

            Flights = await _flightService.FilterFlightsAsync(OriginId, DestinationId, DepartureDate, TotalPassengers);
            var originLocation = await _locationService.GetLocationByIdAsync(OriginId);
            OriginLocation = originLocation.LocationName;
            var destinationLocation = await _locationService.GetLocationByIdAsync(DestinationId);
            DestinationLocation = destinationLocation.LocationName;
        }

        public IActionResult OnPost(decimal basePrice, int flightId, string classType)
        {
            var flightData = HttpContext.Session.GetObjectFromJson<FlightData>("FlightData");

            if (flightData != null)
            {
                flightData.TotalPrice = basePrice * (flightData.AdultNum + flightData.ChildNum * 0.9m + flightData.BabyNum * 0.1m);
                flightData.OutboundFlightId = flightId;
                flightData.ClassType = classType;
            }

            HttpContext.Session.SetObjectAsJson("FlightData", flightData);

            if (flightData.IsOneWay)
            {
                return RedirectToPage("/CRUD/BookingManager/Create");
            }

            return RedirectToPage("./ReturnFlights");
        }
    }
}
