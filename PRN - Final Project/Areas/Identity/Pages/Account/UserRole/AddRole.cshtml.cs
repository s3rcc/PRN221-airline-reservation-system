// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using BussinessObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace PRN___Final_Project.Areas.Identity.Pages.Account.UserRole
{
    public class AddRoleModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AddRoleModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [TempData]
        public string StatusMessage { get; set; }
        [BindProperty]
        [DisplayName("Các role gắn cho user")]        
        public string[] RoleNames { get; set; }
        public User user { get; set; }
        public SelectList allRoles { get; set; }
        public async Task<IActionResult> OnGetAsync(string id)
        {
            if ( !User.IsInRole("Admin"))
            {
                return RedirectToPage("/Errors/404");
            }
            if (string.IsNullOrEmpty(id))
            {
                return NotFound($"Không có user");
            }    
            user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound($"Không thấy user, id = {id},");
            }
            RoleNames =  (await _userManager.GetRolesAsync(user)).ToArray<string>();
            // sua lai
            var roleNames = await _roleManager.Roles.Select(x => x.Name).ToListAsync();
             allRoles = new SelectList(roleNames);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound($"Không có user");
            }
            user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound($"Không thấy user, id = {id},");
            }
            var ownedRoleNames = (await _userManager.GetRolesAsync(user)).ToArray();
            var deleteRoles = ownedRoleNames.Where(r => !RoleNames.Contains(r));
            var addRoles = RoleNames.Where(r => !ownedRoleNames.Contains(r));
            var roleNames = await _roleManager.Roles.Select(x => x.Name).ToListAsync();
            allRoles = new SelectList(roleNames);
            var resultDelete =  await _userManager.RemoveFromRolesAsync(user,deleteRoles);
            if (!resultDelete.Succeeded)
            {
                resultDelete.Errors.ToList().ForEach(
                  x =>
                  {
                      ModelState.AddModelError(string.Empty, x.Description);
                  });
                return Page();
            }
            var resultAdd = await _userManager.AddToRolesAsync(user, addRoles);
            if (!resultAdd.Succeeded)
            {
                resultAdd.Errors.ToList().ForEach(
                  x =>
                  {
                      ModelState.AddModelError(string.Empty, x.Description);
                  });
                return Page();
            }
            StatusMessage = $"Vừa cập nhật role cho user: {user.UserName}";
            return RedirectToPage("./Index");


          
        }
    }
}
