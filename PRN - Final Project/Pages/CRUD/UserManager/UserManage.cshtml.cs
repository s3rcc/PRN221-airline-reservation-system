using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using BussinessObjects;

namespace MyProject.Pages.CRUD.UserManager
{
    public class UserManagementModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly ITierService _tierService;

        public UserManagementModel(IUserService userService, ITierService tierService)
        {
            _tierService = tierService;
            _userService = userService;
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

        public async Task OnGetAsync()
        {

            Users = await _userService.GetAllUsersAsync();
            Tiers = await _tierService.GetAllTiersAsync();
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
                    await _userService.ChangeUserTier(int.Parse(User.Id), User.Tier);
                    Message = "Change user tier successfully!";
                    IsSuccess = true;
                    await Console.Out.WriteLineAsync("\n\n\n +++");
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
