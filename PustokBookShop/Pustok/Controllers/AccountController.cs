using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Pustok.DAL;
using Pustok.Models;
using Pustok.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IHubContext<ChatHub> _hubContext;

        public AccountController(AppDbContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IHubContext<ChatHub> hubContext)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _hubContext = hubContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserRegisterVM registerVM)
        {
            if (!ModelState.IsValid) return View();

            if (_userManager.Users.Any(x=>x.NormalizedUserName == registerVM.Username.ToUpper()))
            {
                ModelState.AddModelError("Username", "This Username is using!");
                return View();
            }

            if (_userManager.Users.Any(x => x.NormalizedEmail == registerVM.Email.ToUpper()))
            {
                ModelState.AddModelError("Email", "This Email is registred!");
                return View();
            }

            AppUser appUser = new AppUser()
            {
                FullName = registerVM.FullName,
                UserName = registerVM.Username,
                Email = registerVM.Email,
                IsAdmin = false
            };

            IdentityResult result =  await _userManager.CreateAsync(appUser, registerVM.Password);

            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return View();
            }

            await _userManager.AddToRoleAsync(appUser, "Member");

            await _signInManager.SignInAsync(appUser, true);

            return RedirectToAction("index","home");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("index", "home");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserLoginVM userL)
        {
            AppUser user = await _userManager.FindByNameAsync(userL.Username);

            if (user == null || user.IsAdmin)
            {
                ModelState.AddModelError("", "Username or Password is false!");
                return View();
            }

            var result =  await _signInManager.PasswordSignInAsync(user, userL.Password, true, false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Username or Password is false!");
                return View();
            }

            return RedirectToAction("index", "home");
        }

        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Edit(int page=1)
        {
            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            UserUpdateVM updateVM = new UserUpdateVM()
            {
                Email = user.Email,
                FullName = user.FullName,
                UserName = user.UserName,
                Country = user.Country,
                City = user.City,
                State = user.State,
                Address = user.Address
            };

            List<Order> orders = _context.Orders.Include(x => x.OrderItems).Where(x => x.AppUserId == user.Id).ToList();

            if (orders != null)
            {
                int totalOrders = orders.Count();
                ViewBag.SelectedPage = page;
                ViewBag.TotalPages = (int)Math.Ceiling(totalOrders / 5d);
                ViewBag.TotalOrders = totalOrders;
                updateVM.Orders = orders.Skip((page - 1) * 5).Take(5).ToList();
            }

            return View(updateVM);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserUpdateVM updateVM)
        {
            if (!ModelState.IsValid) return View();

            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (user.Email != updateVM.Email && _userManager.Users.Any(x=>x.NormalizedEmail == updateVM.Email.ToUpper()))
            {
                ModelState.AddModelError("Email", "Email alredy using!");
                return View();
            }

            if (user.UserName != updateVM.UserName && _userManager.Users.Any(x => x.NormalizedUserName == updateVM.UserName.ToUpper()))
            {
                ModelState.AddModelError("UserName", "UserName alredy using!");
                return View();
            }

            user.Email = updateVM.Email;
            user.UserName = updateVM.UserName;
            user.FullName = updateVM.FullName;
            user.Country = updateVM.Country;
            user.City = updateVM.City;
            user.State = updateVM.State;
            user.Address = updateVM.Address;

            await _userManager.UpdateAsync(user);
            await _signInManager.SignInAsync(user, true);

            return RedirectToAction("index", "home");
        }

        [Authorize(Roles = "Member")]
        public async Task<IActionResult> OrderDetail(int id)
        {
            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            Order order = _context.Orders.Include(x => x.OrderItems).ThenInclude(x=>x.Book).FirstOrDefault(x => x.Id == id && x.AppUserId == user.Id);

            if (order == null) return RedirectToAction("index", "notfound");

            return View(order);
        }

        public IActionResult Changepassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Changepassword(ChangePasswordVM passwordVM)
        {
            if (!ModelState.IsValid) return View();

            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            var result = await _userManager.ChangePasswordAsync(user, passwordVM.OldPassword, passwordVM.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return View();
            }

            return RedirectToAction("index","home");
        }

        public IActionResult Chat()
        {
            if (!User.Identity.IsAuthenticated) return RedirectToAction("login");
            return View();
        }

        
        public async Task<IActionResult> ChatMessage(string connectionId, string message)
        {
            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            List<string> connectionIds = new List<string>();
            connectionIds.Add(user.ConnectionId);
            if (!string.IsNullOrWhiteSpace(connectionId))
            {
                connectionIds.Add(connectionId);
            }
            if (connectionIds.Count == 1)
            {
                await _hubContext.Clients.All.SendAsync("ReceiveMessage", user.FullName, message);
            }
            else
            {
                await _hubContext.Clients.Clients(connectionIds).SendAsync("ReceiveMessage", user.FullName, message);
            }
            return RedirectToAction("chat");
        }
    }
}
