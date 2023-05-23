using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
    public class AuthorController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AuthorController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index(int page = 1)
        {
            int totalAuthors = _context.Authors.Count();
            ViewBag.SelectedPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalAuthors / 2d);
            ViewBag.TotalAuthors = totalAuthors;
            List<Author> authors = _context.Authors.Include(x => x.Books)
                .Skip((page - 1) * 2).Take(2).ToList();

            return View(authors);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Author author)
        {
            if (!ModelState.IsValid) return View();

            if (author.ImageFile != null)
            {
                if(author.ImageFile.ContentType != "image/jpeg" && author.ImageFile.ContentType != "image/png")
                {
                    ModelState.AddModelError("ImageFile", "You can choose file only .jpg, .jpeg or .png format!");
                    return View();
                }

                if (author.ImageFile.Length > 2097152)
                {
                    ModelState.AddModelError("ImageFile", "You can choose file only maximum 2Mb !");
                    return View();
                }

                author.Image = FileManager.Save(_env.WebRootPath, "uploads/authors", author.ImageFile);

                //author.Image = Guid.NewGuid().ToString() +"_"+ author.ImageFile.FileName;
                //string rootPath = @"C:\Users\GabilGG\Documents\CodeWorks\02. HW_Code\HW_210727\Pustok\wwwroot\";
                //string path = rootPath + "uploads/authors/" + author.ImageFile.FileName;
                //string path = Path.Combine(_env.WebRootPath, "uploads/authors", author.Image);

                //FileStream stream = new FileStream(path, FileMode.Create);

                //author.ImageFile.CopyTo(stream);
            }
            
            _context.Authors.Add(author);
            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Update(int id,int page)
        {

            Author author = _context.Authors.FirstOrDefault(x => x.Id == id);

            if (author == null)
            {
                Response.StatusCode = 404;
                return View("AuthorNotFound", id);
            }

            ViewBag.SelectedPage = page;
            return View(author);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Author author, int selectedpage)
        {
            Author existAuthor = _context.Authors.FirstOrDefault(x => x.Id == author.Id);

            if (existAuthor == null)
            {
                Response.StatusCode = 404;
                return View("TagNotFound", author.Id);
            }

            existAuthor.FullName = author.FullName;
            existAuthor.BioDesc = author.BioDesc;

            if (author.ImageFile != null)
            {
                if (author.ImageFile.ContentType != "image/jpeg" && author.ImageFile.ContentType != "image/png")
                {
                    ModelState.AddModelError("ImageFile", "Yalniz .jpg , .jpeg ve ya .png formatda fayl sece bilersiz!");
                    return View();
                }
                if (author.ImageFile.Length> 2097152)
                {
                    ModelState.AddModelError("ImageFile", "Maksimum uzunlugu 2Mb olan fayl sece bilersiz!");
                    return View();
                }

                string newFileName = FileManager.Save(_env.WebRootPath, "uploads/authors", author.ImageFile);

                if (!string.IsNullOrWhiteSpace(existAuthor.Image))
                {
                    FileManager.Delete(_env.WebRootPath, "uploads/authors", existAuthor.Image);

                    //string oldFilePath = Path.Combine(_env.WebRootPath, "uploads/authors", existAuthor.Image);
                    //if (System.IO.File.Exists(oldFilePath))
                    //{
                    //    System.IO.File.Delete(oldFilePath);
                    //}
                }

                existAuthor.Image = newFileName;
            }
            else if(string.IsNullOrWhiteSpace(author.Image) && !string.IsNullOrWhiteSpace(existAuthor.Image))
            {

                FileManager.Delete(_env.WebRootPath, "uploads/authors", existAuthor.Image);

                //string oldFileName = Path.Combine(_env.WebRootPath, "uploads/authors", existAuthor.Image);

                //if (System.IO.File.Exists(oldFileName))
                //{
                //    System.IO.File.Delete(oldFileName);
                //}

                existAuthor.Image = null;
            }
            int page = selectedpage;

            _context.SaveChanges();

            return RedirectToAction("index", page);
        }

        public IActionResult Delete(int id)
        {
            Author existAuthor = _context.Authors.FirstOrDefault(x => x.Id == id);

            if (existAuthor == null)
            {
                Response.StatusCode = 404;
                return View("TagNotFound", id);
            }

            return View(existAuthor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Author author)
        {
            Author existAuthor = _context.Authors.FirstOrDefault(x => x.Id == author.Id);

            if (existAuthor == null)
            {
                Response.StatusCode = 404;
                return View("TagNotFound", author.Id);
            }

            if (string.IsNullOrWhiteSpace(existAuthor.Image))
            {
                FileManager.Delete(_env.WebRootPath, "uploads/authors", existAuthor.Image);

                //string fileName = Path.Combine(_env.WebRootPath, "uploads/authors", existAuthor.Image);

                //if (System.IO.File.Exists(fileName))
                //{
                //    System.IO.File.Delete(fileName);
                //}
            }

            _context.Authors.Remove(existAuthor);
            _context.SaveChanges();

            return RedirectToAction("index");
        }
    }
}
