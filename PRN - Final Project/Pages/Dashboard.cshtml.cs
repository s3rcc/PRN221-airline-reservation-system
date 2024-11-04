using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using BussinessObjects;

namespace PRN___Final_Project.Pages
{
    public class DashboardModel : PageModel
    {
        private readonly UserManager<User> _userManager;

        public DashboardModel(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Errors/404");
            }
            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains("Admin") && !roles.Contains("Staff"))
            {
                return RedirectToPage("/Errors/404");
            }

            return Page();
        }
    }
}
