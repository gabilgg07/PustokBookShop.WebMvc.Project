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
    public class TagController : Controller
    {
        private readonly AppDbContext _context;

        public TagController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int page=1)
        {
            int totalTags = _context.Tags.Count();
            ViewBag.SelectedPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalTags / 2d);
            ViewBag.TotalTags = totalTags;
            List<Tag> tags = _context.Tags.Include(x => x.BookTags)
                .ThenInclude(x=>x.Book)
                .Skip((page - 1) * 2).Take(2).ToList();

            return View(tags);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Tag tag)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            _context.Tags.Add(tag);
            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Update(int id)
        {
            Tag tag = _context.Tags.FirstOrDefault(x => x.Id == id);

            if (tag == null)
            {
                Response.StatusCode = 404;
                return View("TagNotFound", id);
            }

            return View(tag);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Tag tag)
        {
            Tag existTag = _context.Tags.FirstOrDefault(x => x.Id == tag.Id);

            if (existTag == null)
            {
                Response.StatusCode = 404;
                return View("TagNotFound", tag.Id);
            }

            existTag.Name = tag.Name;
            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Delete(int id)
        {
            Tag existTag = _context.Tags.FirstOrDefault(x => x.Id == id);

            if (existTag == null)
            {
                return Json(new { status = 404 });
            }

            _context.Tags.Remove(existTag);
            _context.SaveChanges();

            return Json(new { status = 200 });
        }
    }
}
