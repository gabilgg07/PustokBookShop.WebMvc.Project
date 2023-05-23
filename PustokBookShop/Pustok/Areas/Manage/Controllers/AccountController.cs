using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pustok.Areas.Manage.ViewModels;
using Pustok.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        //public async Task<IActionResult> CreateAdmin()
        //{
        //    AppUser appUser = new AppUser()
        //    {
        //        UserName = "superadmin",
        //        FullName = "Qabil SuperAdmin"
        //    };

        //    await _userManager.CreateAsync(appUser, "super123");
        //    await _userManager.AddToRoleAsync(appUser, "SuperAdmin");

        //    return Ok();
        //}

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AdminLoginVM loginVM)
        {
            if (!ModelState.IsValid) return View(loginVM);

            AppUser admin = await _userManager.FindByNameAsync(loginVM.UserName);

            if (admin == null || !admin.IsAdmin)
            {
                ModelState.AddModelError("", "Username or Password is false!");
                return View(loginVM);
            }

            var result = await _signInManager.PasswordSignInAsync(admin, loginVM.Password, true, false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Username or Password is false!");
                return View(loginVM);
            }

            return RedirectToAction("index", "dashboard");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("login");
        }
    }
}
