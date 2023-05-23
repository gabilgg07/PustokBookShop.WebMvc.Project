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
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int page=1)
        {
            int totalCtgs = _context.Categories.Count();
            ViewBag.SelectedPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalCtgs/2d);
            ViewBag.TotalCategories = totalCtgs;
            List<Category> categories = _context.Categories.Include(x=>x.Books).Skip((page-1)*2).Take(2).ToList();

            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            _context.Categories.Add(category);
            _context.SaveChanges();
           
            return RedirectToAction("index");
        }

        public IActionResult Update(int id)
        {
            Category category = _context.Categories.FirstOrDefault(x => x.Id == id);

            if (category==null)
            {
                Response.StatusCode = 404;
                return View("CategoryNotFound", id);
            }

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Category category)
        {
            Category existCategory = _context.Categories.FirstOrDefault(x => x.Id == category.Id);

            if (existCategory == null)
            {
                Response.StatusCode = 404;
                return View("CategoryNotFound", category.Id);
            }

            existCategory.Name = category.Name;
            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Delete(int id)
        {
            Category existCategory = _context.Categories.FirstOrDefault(x => x.Id == id);

            if (existCategory == null)
            {
                return Json(new {status = 404 });
            }

            _context.Categories.Remove(existCategory);
            _context.SaveChanges();

            return Json(new { status = 200 });
        }
    }
}
