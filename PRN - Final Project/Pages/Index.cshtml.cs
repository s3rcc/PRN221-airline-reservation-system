using BussinessObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interfaces;

namespace PRN___Final_Project.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ILocationService _locationService;
        private readonly UserManager<User> _userManager;

        public IndexModel(ILogger<IndexModel> logger, ILocationService locationService, UserManager<User> userManager)
        {
            _logger = logger;
            _locationService = locationService;
            _userManager = userManager;
        }

        public IEnumerable<Location> Locations { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                if (await _userManager.IsInRoleAsync(user, "Admin") || await _userManager.IsInRoleAsync(user, "Staff"))
                {
                    return RedirectToPage("/Dashboard");
                }
            }
            Locations = await _locationService.GetAllLocationsAsync();
            return Page();
        }

        public IActionResult OnPost(string originId, string destinationId, DateTime departureDate, DateTime returnDate, int totalPassengers, bool isOneWay, int adultNum, int childNum, int babyNum)
        {
            var flightData = new FlightData
            {
                OriginId = int.Parse(originId),
                DestinationId = int.Parse(destinationId),
                DepartureDate = departureDate,
                ReturnDate = returnDate,
                TotalPassengers = totalPassengers,
                IsOneWay = isOneWay,
                AdultNum = adultNum,
                ChildNum = childNum,
                BabyNum = babyNum
            };

            HttpContext.Session.SetObjectAsJson("FlightData", flightData);
            HttpContext.Session.Remove("BookingId");

            return RedirectToPage("/CRUD/FlightManager/OutboundFlights");
        }
    }
}
