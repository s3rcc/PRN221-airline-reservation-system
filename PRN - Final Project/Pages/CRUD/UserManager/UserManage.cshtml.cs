using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using BussinessObjects;
using Microsoft.AspNetCore.Identity;

namespace MyProject.Pages.CRUD.UserManager
{
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
            Users = new List<User>();
            Tiers = new List<Tier>();
            User = new User();
        }

        [BindProperty]
        public User User { get; set; }
        public IEnumerable<User> Users { get; set; }
        public IEnumerable<Tier> Tiers { get; set; }

        // Add properties for message display
        public string Message { get; set; }
        public bool IsSuccess { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            //var user = await _userManager.GetUserAsync(User);
            //if (user == null || !await _userManager.IsInRoleAsync(user, "admin"))
            //{
            //    // Trả về trang 404 nếu người dùng không phải là admin
            //    return RedirectToPage("/Errors/404");
            //}

            Users = await _userService.GetAllUsersAsync();
            Tiers = await _tierService.GetAllTiersAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await Console.Out.WriteLineAsync("___ Post ___");
            //if (!ModelState.IsValid)
            //{
            //    return Page();
            //}

            if (string.IsNullOrEmpty(User.Id))
            {
                await _userService.AddUserAsync(User);
            }
            else
            {
                await Console.Out.WriteLineAsync("___ Update ___");

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
                    Message = "Error change user tier!";
                    IsSuccess = false;
                    await Console.Out.WriteLineAsync("\n\n\n ---");
                }
                await Console.Out.WriteLineAsync("### Update ###");
            }
            await Console.Out.WriteLineAsync("### Post ###");

            return RedirectToPage(new { successMessage = Message, isSuccess = IsSuccess }); ;
        }

        //public async Task<IActionResult> OnPostDeleteAsync(int id)
        //{
        //    await Console.Out.WriteLineAsync("\n\n\n\n=== Get id: "+ id);

        //    await _userService.DeleteUserAsync(id);
        //    return RedirectToPage();
        //}
    }
}
