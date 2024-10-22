using BussinessObjects;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interfaces;

namespace PRN___Final_Project.Pages.CRUD.FlightManager
{
    public class ReturnFlightsModel : PageModel
    {
        private readonly IFlightService _flightService;
        private readonly ILocationService _locationService;

        public ReturnFlightsModel(IFlightService flightService, ILocationService locationService)
        {
            _flightService = flightService;
            _locationService = locationService;
        }

        public IEnumerable<Flight> Flights { get; set; } = default!;
        public decimal TotalFare { get; set; }
        public int OriginId { get; set; }
        public int DestinationId { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public int TotalPassengers { get; set; }
        public string OriginLocation { get; set; }
        public string DestinationLocation { get; set; }

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
            }

            Flights = await _flightService.FilterFlightsAsync(DestinationId, OriginId, ReturnDate);
            var originLocation = await _locationService.GetLocationByIdAsync(DestinationId);
            OriginLocation = originLocation.LocationName;
            var destinationLocation = await _locationService.GetLocationByIdAsync(OriginId);
            DestinationLocation = destinationLocation.LocationName;

            TotalFare = CalculateTotalFare(Flights);
        }

        private decimal CalculateTotalFare(IEnumerable<Flight> flights)
        {
            return flights.Sum(f => f.BasePrice);
        }
    }
}
