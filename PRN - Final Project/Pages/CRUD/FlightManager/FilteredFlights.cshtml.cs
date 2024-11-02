using BussinessObjects;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interfaces;

namespace PRN___Final_Project.Pages.CRUD.FlightManager
{
    public class FilteredFlightsModel : PageModel
    {
        private readonly IFlightService _flightService;

        public FilteredFlightsModel(IFlightService flightService)
        {
            _flightService = flightService;
        }

        public IEnumerable<Flight> OutboundFlights { get; set; } = default!;
        public IEnumerable<Flight> ReturnFlights { get; set; } = default!;
        public decimal TotalFare { get; set; }

        public async Task OnGetAsync(int originId, int destinationId, DateTime departureDate, DateTime? returnDate = null)
        {
            // L?y chuy?n bay ?i
            //OutboundFlights = await _flightService.FilterFlightsAsync(originId, destinationId, departureDate);

            // N?u có ngày v?, l?y chuy?n bay v?
            if (returnDate.HasValue)
            {
                //ReturnFlights = await _flightService.FilterFlightsAsync(destinationId, originId, returnDate.Value);
                // Tính t?ng ti?n vé (ví d? gi? ??nh m?i vé 100)
                TotalFare = (OutboundFlights.Count() + ReturnFlights.Count()) * 100; // Thay ??i theo logic c?a b?n
            }
        }
    }
}
