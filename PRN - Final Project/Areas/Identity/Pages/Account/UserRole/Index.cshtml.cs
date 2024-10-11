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
    //    [Authorize](Roles = "Admin")

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

        // ??i t??ng ch?a thông tin ng??i dùng và vai trò
        public IEnumerable<UserWithRoles> UsersWithRoles { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        public async Task OnGetAsync(int pageNumber = 1)
        {
            int pageSize = 10; // S? l??ng ng??i dùng trên m?i trang
            var result = await _userService.GetAllUsersPagedAsync(pageNumber, pageSize);
            var users = result.Users;
            int totalRecords = result.TotalCount;

            // L?y danh sách vai trò c?a m?i ng??i dùng
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
        }
    }

    // L?p ?? ch?a thông tin ng??i dùng và vai trò c?a h?
    public class UserWithRoles
    {
        public User User { get; set; }
        public List<string> Roles { get; set; }
    }
}
