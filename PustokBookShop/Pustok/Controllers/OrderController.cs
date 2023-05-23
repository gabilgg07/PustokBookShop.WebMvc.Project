using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public OrderController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles ="Member")]
        public async Task<IActionResult> CheckOut()
        {
            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            OrderCreateVM orderCreateVM = new OrderCreateVM()
            {
                Country = user.Country,
                City = user.City,
                State = user.State,
                Address = user.Address,
                BasketItems = _context.BasketItems.Include(x=>x.Book).Where(x => x.AppUserId == user.Id).ToList()
            };

            return View(orderCreateVM);
        }

        [Authorize(Roles ="Member")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckOut(OrderCreateVM orderVM)
        {
            if (!ModelState.IsValid) return RedirectToAction("checkout");

            AppUser user =await _userManager.FindByNameAsync(User.Identity.Name);

            List<BasketItem> basketItems = await _context.BasketItems.Include(x => x.Book).Where(x => x.AppUserId == user.Id).ToListAsync();

            if (basketItems.Count() == 0)
            {
                TempData["Error"] = "Səbət boşdur!";
                return RedirectToAction("checkout");
            }


            Order order = new Order
            {
                Address = orderVM.Address,
                Country = orderVM.Country,
                City = orderVM.City,
                State = orderVM.State,
                Note = orderVM.Note,
                AppUserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                OrderItems = new List<OrderItem>()
            };

            foreach (var basketItem in basketItems)
            {
                OrderItem orderItem = new OrderItem
                {
                    BookId = basketItem.BookId,
                    Count = basketItem.Count,
                    Name = basketItem.Book.Name
                };

                orderItem.Price = basketItem.Book.DiscountedPrice == null ?basketItem.Book.Price :(double)basketItem.Book.DiscountedPrice;

                order.OrderItems.Add(orderItem);
                order.TotalPrice += orderItem.Price * orderItem.Count;
            }

            await _context.Orders.AddAsync(order);

            _context.BasketItems.RemoveRange(basketItems);
            await _context.SaveChangesAsync();

            TempData["Succeed"] = "Sifariş uğurla yaradıldı!";

            return RedirectToAction("index", "home");
        }

    }
}
