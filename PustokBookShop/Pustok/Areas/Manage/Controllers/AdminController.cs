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
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Index()
        {

            List<AppUser> admins = (List<AppUser>)await _userManager.GetUsersInRoleAsync("Admin");

            return View(admins);
        }

        [Authorize(Roles = "SuperAdmin")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminCreateVM adminVM)
        {
            if (_userManager.Users.Any(x=>x.UserName== adminVM.UserName))
            {
                ModelState.AddModelError("UserName", "Bu username istifade edilir!");
                return View();
            }

            if (_userManager.Users.Any(x => x.Email == adminVM.Email))
            {
                ModelState.AddModelError("Email", "Bu email istifade edilir!");
                return View();
            }

            AppUser admin = new AppUser()
            {
                FullName = adminVM.FullName,
                Email = adminVM.Email,
                UserName = adminVM.UserName,
                IsAdmin = true
            };

            await _userManager.CreateAsync(admin, adminVM.Password);

            await _userManager.AddToRoleAsync(admin, "Admin");

            return RedirectToAction("index");
        }

        [Authorize(Roles = "SuperAdmin")]
        public IActionResult Addrole()
        {
            return Content("Hele hazir deyil))))");
        }

        public async  Task<IActionResult> Edit(string id)
        {
            AppUser admin = await _userManager.FindByIdAsync(id);

            AppUser superAdmin = _userManager.GetUsersInRoleAsync("SuperAdmin").Result.FirstOrDefault();

            AppUser identityAdmin = await _userManager.FindByNameAsync(User.Identity.Name);

            if (admin == null) return View("AdminNotFound", id);

            if (admin != identityAdmin && identityAdmin != superAdmin) return View("AdminNotFound", id);

            AdminEditVM adminEditVM = new AdminEditVM()
            {
                Id = admin.Id,
                FullName = admin.FullName,
                UserName = admin.UserName,
                Email = admin.Email,
            };

            return View(adminEditVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AdminEditVM adminEditVM)
        {
            if (!ModelState.IsValid) return View();

            AppUser existAdmin = await _userManager.FindByIdAsync(adminEditVM.Id);

            if (existAdmin == null) return View("AdminNotFound", adminEditVM.Id);

            if (_userManager.Users.Any(x=>x.UserName == adminEditVM.UserName) && existAdmin.UserName != adminEditVM.UserName)
            {
                ModelState.AddModelError("UserName", "Bu username istifade edilir!");
                return View();
            }
            if (_userManager.Users.Any(x => x.Email == adminEditVM.Email) && existAdmin.Email != adminEditVM.Email)
            {
                ModelState.AddModelError("Email", "Bu e-mail istifade edilir!");
                return View();
            }

            if (adminEditVM.Password != null)
            {
                if (adminEditVM.Password != adminEditVM.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword","Sifre tekrari duzgun yazilmayib!");
                    return View();
                }
                var token = await _userManager.GeneratePasswordResetTokenAsync(existAdmin);


                 var result = await _userManager.ResetPasswordAsync(existAdmin, token, adminEditVM.Password);

                if (!result.Succeeded)
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                        return View();
                    }
                }

            }

            existAdmin.FullName = adminEditVM.FullName;
            existAdmin.UserName = adminEditVM.UserName;
            existAdmin.Email = adminEditVM.Email;

            await _userManager.UpdateAsync(existAdmin);

            return RedirectToAction("index");
        }


        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(string id)
        {
            AppUser admin = await _userManager.FindByIdAsync(id);

            if (admin == null) return View("AdminNotFound", id);

            await _userManager.DeleteAsync(admin);

            return RedirectToAction("index");
        }

    }
}
