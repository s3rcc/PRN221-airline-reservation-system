using BussinessObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN___Final_Project.Areas.Identity.Pages.Account.Manage
{
    [Authorize]
    public class ListBookingModel : PageModel
    {
        private readonly IBookingService _bookingService;
        private readonly UserManager<User> _userManager;

        public ListBookingModel(IBookingService bookingService, UserManager<User> userManager)
        {
            _bookingService = bookingService;
            _userManager = userManager;
            Bookings = new List<Booking>();
        }

        public IEnumerable<Booking> Bookings { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("Unable to load user.");
            }
            if (!await _userManager.IsInRoleAsync(user, "Member"))
            {
                return RedirectToPage("/Errors/404"); 
            }

            Bookings = await _bookingService.GetBookingByUserIdAsync(user.Id);
            return Page();
        }
    }
}
