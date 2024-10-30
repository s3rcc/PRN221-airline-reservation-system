using BussinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interfaces;

namespace PRN___Final_Project.Pages.CRUD.FlightManager
{
    public class OutboundFlightsModel : PageModel
    {
        private readonly IFlightService _flightService;
        private readonly ILocationService _locationService;

        public OutboundFlightsModel(IFlightService flightService, ILocationService locationService)
        {
            _flightService = flightService;
            _locationService = locationService;
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

        public IActionResult OnPost(decimal basePrice, int flightId)
        {
            var flightData = HttpContext.Session.GetObjectFromJson<FlightData>("FlightData");

            if (flightData != null)
            {
                flightData.TotalPrice = basePrice * flightData.TotalPassengers;
                flightData.OutboundFlightId = flightId;
            }

            HttpContext.Session.SetObjectAsJson("FlightData", flightData);

            if (flightData.IsOneWay)
            {
                return Redirect("/manage-bookings");
            }

            return RedirectToPage("./ReturnFlights");
        }
    }
}
