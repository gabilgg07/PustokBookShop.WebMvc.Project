using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.DAL;
using Pustok.Helpers;
using Pustok.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class SliderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SliderController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index(int page = 1)
        {
            int totalSliders = _context.Sliders.Count();
            ViewBag.SelectedPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalSliders / 2d);
            ViewBag.TotalSliders = totalSliders;
            List<Slider> sliders = _context.Sliders
                .Skip((page - 1) * 2).Take(2).ToList();

            return View(sliders);
        }

        public IActionResult Detail(int id)
        {

            Slider slider = _context.Sliders.FirstOrDefault(x => x.Id == id);

            if (slider == null)
            {
                Response.StatusCode = 404;
                return View("SliderNotFound", id);
            }

            return View(slider);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Slider slider)
        {
            if (!ModelState.IsValid) return View();

            if (slider.ImageFile != null)
            {
                if (slider.ImageFile.ContentType != "image/jpeg" && slider.ImageFile.ContentType != "image/png")
                {
                    ModelState.AddModelError("ImageFile", "You can choose file only .jpg, .jpeg or .png format!");
                    return View();
                }

                if (slider.ImageFile.Length > 5242880)
                {
                    ModelState.AddModelError("ImageFile", "You can choose file only maximum 5Mb !");
                    return View();
                }

                slider.Image = FileManager.Save(_env.WebRootPath, "uploads/sliders", slider.ImageFile);
            }

            _context.Sliders.Add(slider);
            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Update(int id)
        {
            Slider slider = _context.Sliders.FirstOrDefault(x => x.Id == id);

            if (slider == null)
            {
                Response.StatusCode = 404;
                return View("SliderNotFound", id);
            }

            return View(slider);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Slider slider)
        {
            Slider existSlider = _context.Sliders.FirstOrDefault(x => x.Id == slider.Id);

            if (existSlider == null)
            {
                Response.StatusCode = 404;
                return View("SliderNotFound", slider.Id);
            }

            existSlider.Title = slider.Title;
            existSlider.Subtitle = slider.Subtitle;
            existSlider.RedirectUrl = slider.RedirectUrl;
            existSlider.Order = slider.Order;
            existSlider.BtnText = slider.BtnText;

            if (slider.ImageFile != null)
            {
                if (slider.ImageFile.ContentType != "image/jpeg" && slider.ImageFile.ContentType != "image/png")
                {
                    ModelState.AddModelError("ImageFile", "Yalniz .jpg , .jpeg ve ya .png formatda fayl sece bilersiz!");
                    return View();
                }
                if (slider.ImageFile.Length > 5242880)
                {
                    ModelState.AddModelError("ImageFile", "Maksimum uzunlugu 5Mb olan fayl sece bilersiz!");
                    return View();
                }

                string newFileName = FileManager.Save(_env.WebRootPath, "uploads/sliders", slider.ImageFile);

                if (!string.IsNullOrWhiteSpace(existSlider.Image))
                {
                    FileManager.Delete(_env.WebRootPath, "uploads/sliders", existSlider.Image);
                }

                existSlider.Image = newFileName;
            }
            else if (string.IsNullOrWhiteSpace(slider.Image) && !string.IsNullOrWhiteSpace(existSlider.Image))
            {

                FileManager.Delete(_env.WebRootPath, "uploads/sliders", existSlider.Image);

                existSlider.Image = null;
            }

            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Delete(int id)
        {
            Slider existSlider = _context.Sliders.FirstOrDefault(x => x.Id == id);

            if (existSlider == null)
            {
                Response.StatusCode = 404;
                return View("SliderNotFound", id);
            }

            return View(existSlider);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Slider slider)
        {
            Slider existSlider = _context.Sliders.FirstOrDefault(x => x.Id == slider.Id);

            if (existSlider == null)
            {
                Response.StatusCode = 404;
                return View("SliderNotFound", slider.Id);
            }

            if (!string.IsNullOrWhiteSpace(existSlider.Image))
            {
                FileManager.Delete(_env.WebRootPath, "uploads/sliders", existSlider.Image);
            }

            _context.Sliders.Remove(existSlider);
            _context.SaveChanges();

            return RedirectToAction("index");
        }
    }
}
