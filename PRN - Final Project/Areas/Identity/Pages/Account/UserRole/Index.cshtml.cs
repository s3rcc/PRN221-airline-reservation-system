using BussinessObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRN___Final_Project.Areas.Identity.Pages.Account.UserRole
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserService _userService;

        public IndexModel(UserManager<User> userManager, IUserService userService)
        {
            _userManager = userManager;
            _userService = userService;
        }

        [TempData]
        public string StatusMessage { get; set; }

        // Holds user information and their roles
        public IEnumerable<UserWithRoles> UsersWithRoles { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        public async Task<IActionResult> OnGetAsync(int pageNumber = 1)
        {
            if (!User.IsInRole("Admin"))
            {
                return RedirectToPage("/Errors/404");
            }

            int pageSize = 10; // Number of users per page
            var result = await _userService.GetAllUsersPagedAsync(pageNumber, pageSize);
            var users = result.Users;
            int totalRecords = result.TotalCount;

            // Retrieve the roles of each user
            var userWithRolesList = new List<UserWithRoles>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userWithRolesList.Add(new UserWithRoles
                {
                    User = user,
                    Roles = roles.ToList()
                });
            }

            UsersWithRoles = userWithRolesList;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            return Page();
        }
    }

    // Class to hold user information and their roles
    public class UserWithRoles
    {
        public User User { get; set; }
        public List<string> Roles { get; set; }
    }
}
