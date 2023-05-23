using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Pustok.DAL;
using Pustok.Helpers;
using Pustok.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class SettingController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SettingController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index()
        {
            Setting setting = _context.Settings.FirstOrDefault();

            return View(setting);
        }

        public IActionResult Edit()
        {
            Setting setting = _context.Settings.FirstOrDefault();

            return View(setting);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Setting setting)
        {
            if (!ModelState.IsValid) return View();

            Setting existSetting = _context.Settings.FirstOrDefault();

            existSetting.Phone1 = setting.Phone1;
            existSetting.Phone2 = setting.Phone2;
            existSetting.Email = setting.Email;
            existSetting.Address = setting.Address;

            if (setting.HeaderImage != null)
            {
                if (setting.HeaderImage.ContentType != "image/jpeg" && setting.HeaderImage.ContentType != "image/png")
                {
                    ModelState.AddModelError("HeaderImage", "Yalniz .jpg , .jpeg ve ya .png formatda fayl sece bilersiz!");
                    return View();
                }
                if (setting.HeaderImage.Length > 5242880)
                {
                    ModelState.AddModelError("HeaderImage", "Maksimum uzunlugu 5Mb olan fayl sece bilersiz!");
                    return View();
                }

                string newFileName = FileManager.Save(_env.WebRootPath, "image", setting.HeaderImage);

                if (!string.IsNullOrWhiteSpace(existSetting.HeaderLogo))
                {
                    FileManager.Delete(_env.WebRootPath, "image", existSetting.HeaderLogo);
                }

                existSetting.HeaderLogo = newFileName;
            }

            if (setting.FooterImage != null)
            {
                if (setting.FooterImage.ContentType != "image/jpeg" && setting.FooterImage.ContentType != "image/png")
                {
                    ModelState.AddModelError("FooterImage", "Yalniz .jpg , .jpeg ve ya .png formatda fayl sece bilersiz!");
                    return View();
                }
                if (setting.FooterImage.Length > 5242880)
                {
                    ModelState.AddModelError("FooterImage", "Maksimum uzunlugu 5Mb olan fayl sece bilersiz!");
                    return View();
                }

                string newFileName = FileManager.Save(_env.WebRootPath, "image", setting.FooterImage);

                if (!string.IsNullOrWhiteSpace(existSetting.FooterLogo))
                {
                    FileManager.Delete(_env.WebRootPath, "image", existSetting.FooterLogo);
                }

                existSetting.FooterLogo = newFileName;
            }
            else if (string.IsNullOrWhiteSpace(setting.FooterLogo) && !string.IsNullOrWhiteSpace(existSetting.FooterLogo))
            {

                FileManager.Delete(_env.WebRootPath, "image", existSetting.FooterLogo);

                existSetting.FooterLogo = null;
            }

            _context.SaveChanges();

            return RedirectToAction("index","setting");
        }
    }
}
