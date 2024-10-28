using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PRN___Final_Project.Pages.CRUD.FlightManager
{
    public class FlightDetailsModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public bool IsOneWay { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Origin { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Destination { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime DepartureDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? ReturnDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public int TotalPassengers { get; set; }

        public void OnGet()
        {
            // Any additional logic for handling flight details can go here
        }
    }
}
