using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using BussinessObjects;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace MyProject.Pages.CRUD.UserManager
{
    [Authorize(Roles = "Admin")]
    public class UserManagementModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly ITierService _tierService;
        private readonly UserManager<User> _userManager;

        public UserManagementModel(IUserService userService, ITierService tierService, UserManager<User> userManager)
        {
            _tierService = tierService;
            _userService = userService;
            _userManager = userManager;
            UsersWithRoles = new List<UserWithRolesViewModel>();
            Tiers = new List<Tier>();
        }

        [BindProperty]
        public User User { get; set; }
        public List<UserWithRolesViewModel> UsersWithRoles { get; set; }
        public IEnumerable<Tier> Tiers { get; set; }

        // Add properties for message display
        public string Message { get; set; }
        public bool IsSuccess { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var users = await _userService.GetAllUsersAsync();
            Tiers = await _tierService.GetAllTiersAsync();

            // Populate UsersWithRoles
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                UsersWithRoles.Add(new UserWithRolesViewModel
                {
                    User = user,
                    Roles = roles
                });
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(User.Id))
            {
                await _userService.AddUserAsync(User);
            }
            else
            {
                try
                {
                    var existingUser = await _userManager.FindByIdAsync(User.Id);
                    if (existingUser == null)
                    {
                        Message = "User not found!";
                        IsSuccess = false;
                        return Page();
                    }

                    existingUser.Tier = User.Tier;
                    existingUser.TierId = User.TierId;

                    var result = await _userManager.UpdateAsync(existingUser);
                    if (result.Succeeded)
                    {
                        Message = "User updated successfully!";
                        IsSuccess = true;
                    }
                    else
                    {
                        Message = "Failed to update user!";
                        IsSuccess = false;
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                catch
                {
                    Message = "Error changing user tier!";
                    IsSuccess = false;
                }
            }

            return RedirectToPage(new { successMessage = Message, isSuccess = IsSuccess });
        }

        // ViewModel for User with Roles
        public class UserWithRolesViewModel
        {
            public User User { get; set; }
            public IList<string> Roles { get; set; }
        }
    }
}
