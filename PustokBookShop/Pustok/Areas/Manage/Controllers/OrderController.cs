using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.DAL;
using Pustok.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int page = 1)
        {

            int totalOrders = _context.Orders.Count();
            ViewBag.SelectedPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalOrders / 2d);
            ViewBag.TotalOrders = totalOrders;
            List<Order> orders = _context.Orders.Include(x => x.OrderItems)
                .Include(x => x.AppUser)
                .Skip((page - 1) * 2).Take(2).ToList();

            return View(orders);
        }

        public IActionResult Detail(int id, int page)
        {
            Order order = _context.Orders.Include(x=>x.AppUser).Include(x=>x.OrderItems).ThenInclude(x=>x.Book).FirstOrDefault(x => x.Id == id);

            if (order == null) return View("OrderNotFound", id);
           
            ViewBag.SelectedPage = page;
            
            return View(order);
        }

        public IActionResult Accept(int id, string note)
        {
            Order order = _context.Orders.FirstOrDefault(x => x.Id == id);

            if (order == null) return Json(new { status = 404 });

            order.Status = true;
            order.AdminNote = note;

            _context.SaveChanges();

            return Json(new { status = 200 });
        }

        public IActionResult Reject(int id, string note)
        {
            Order order = _context.Orders.FirstOrDefault(x => x.Id == id);

            if (order == null) return Json(new { status = 404 });

            if (note== null)
            {
               return Json(new { status = 400 });
            }

            order.Status = false;
            order.AdminNote = note;

            _context.SaveChanges();

            return Json(new { status = 200 });
        }
    }
}
