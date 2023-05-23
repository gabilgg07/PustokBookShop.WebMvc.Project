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
    public class PublisherController : Controller
    {
        private readonly AppDbContext _context;

        public PublisherController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int page=1)
        {
            int totalPbshs = _context.Publishers.Count();
            ViewBag.SelectedPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalPbshs / 2d);
            ViewBag.TotalPublishers = totalPbshs;
            List<Publisher> publishers = _context.Publishers.Include(x => x.Books).Skip((page - 1) * 2).Take(2).ToList();

            return View(publishers);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Publisher publisher)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            _context.Publishers.Add(publisher);
            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Update(int id )
        {
            Publisher publisher = _context.Publishers.FirstOrDefault(x => x.Id == id);

            if (publisher == null)
            {
                Response.StatusCode = 404;
                return View("PublisherNotFound", id);
            }

            return View(publisher);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Publisher publisher, int? page)
        {
            Publisher existPublisher = _context.Publishers.FirstOrDefault(x => x.Id == publisher.Id);

            if (existPublisher == null)
            {
                Response.StatusCode = 404;
                return View("PublisherNotFound", publisher.Id);
            }

            existPublisher.Name = publisher.Name;
            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Delete(int id)
        {
            Publisher existPublisher = _context.Publishers.FirstOrDefault(x => x.Id == id);

            if (existPublisher == null)
            {
                return Json(new { status = 404 });
            }

            _context.Publishers.Remove(existPublisher);
            _context.SaveChanges();

            return Json(new { status = 200 });
        }
    }
}
