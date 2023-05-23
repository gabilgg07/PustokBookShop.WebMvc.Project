using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.DAL;
using Pustok.Helpers;
using Pustok.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pustok.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class BookController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public BookController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index(int page = 1)
        {
            int totalBooks = _context.Books.Count();
            ViewBag.SelectedPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalBooks / 5d);
            ViewBag.TotalBooks = totalBooks;
            List<Book> books = _context.Books
                .Include(b => b.Author)
                .Include(b => b.Comments)
                .Skip((page - 1) * 5).Take(5).ToList();

            return View(books);
        }

        public IActionResult Detail(int id)
        {

            Book book = _context.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .Include(b => b.Publisher)
                .Include(b => b.Category)
                .Include(b => b.BookImages)
                .Include(b => b.BookTags)
                .ThenInclude(bt => bt.Tag)
                .Include(b => b.CampaignBooks)
                .ThenInclude(cb => cb.Campaign)
                .FirstOrDefault(x => x.Id == id);

            if (book == null)
            {
                Response.StatusCode = 404;
                return View("BookNotFound", id);
            }

            return View(book);
        }

        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genres = _context.Genres.ToList();
            ViewBag.Publishers = _context.Publishers.ToList();
            ViewBag.Tags = _context.Tags.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Book book)
        {
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genres = _context.Genres.ToList();
            ViewBag.Publishers = _context.Publishers.ToList();
            ViewBag.Tags = _context.Tags.ToList();   

            if (!ModelState.IsValid) return View();

            #region AnyCheck

            if (!_context.Categories.Any(x => x.Id == book.CategoryId))
            {
                ModelState.AddModelError("CategoryId", "Have not Category like this name!");
                return View();
            }

            if (!_context.Authors.Any(x => x.Id == book.AuthorId))
            {
                ModelState.AddModelError("AuthorId", "Have not Author like this fullname!");
                return View();
            }

            if (!_context.Genres.Any(x => x.Id == book.GenreId))
            {
                ModelState.AddModelError("GenreId", "Have not Genre like this name!");
                return View();
            }

            if (!_context.Publishers.Any(x => x.Id == book.PublisherId))
            {
                ModelState.AddModelError("PublisherId", "Have not Publisher like this name!");
                return View();
            }

            if (_context.Books.Any(x => x.Code == book.Code))
            {
                ModelState.AddModelError("Code", "Code is required!");
                return View();
            }

            #endregion

            #region ImageChack

            if (book.PosterImage == null)
            {
                ModelState.AddModelError("PosterImage", "Poster image can not be null!");
                return View();
            }

            if (book.HoverPosterImage == null)
            {
                ModelState.AddModelError("PosterImage", "Hover Poster image can not be null!");
                return View();
            }

            if (book.PosterImage.ContentType != "image/jpeg" && book.PosterImage.ContentType != "image/png")
            {
                ModelState.AddModelError("PosterImage", "You can choose file only .jpg, .jpeg or .png format!");
                return View();
            }

            if (book.PosterImage.Length > 5242880)
            {
                ModelState.AddModelError("PosterImage", "You can choose file only maximum 5Mb !");
                return View();
            }

            if (book.HoverPosterImage.ContentType != "image/jpeg" && book.HoverPosterImage.ContentType != "image/png")
            {
                ModelState.AddModelError("HoverPosterImage", "You can choose file only .jpg, .jpeg or .png format!");
                return View();
            }

            if (book.HoverPosterImage.Length > 5242880)
            {
                ModelState.AddModelError("HoverPosterImage", "You can choose file only maximum 5Mb !");
                return View();
            }

            if (book.Images != null)
            {
                foreach (var item in book.Images)
                {
                    if (item.ContentType != "image/jpeg" && item.ContentType != "image/png")
                    {
                        ModelState.AddModelError("Images", "You can choose file only .jpg, .jpeg or .png format!");
                        return View();
                    }

                    if (item.Length > 5242880)
                    {
                        ModelState.AddModelError("Images", "You can choose file only maximum 5Mb !");
                        return View();
                    }
                }
            }

            #endregion

            #region ImageUpload

            book.BookImages = new List<BookImage>();
            book.BookTags = new List<BookTag>();

            BookImage posterImage = new BookImage()
            {
                PosterStatus = true,
                Image = FileManager.Save(_env.WebRootPath, "uploads/books", book.PosterImage)
            };

            book.BookImages.Add(posterImage);

            BookImage hoverPosterImage = new BookImage()
            {
                PosterStatus = false,
                Image = FileManager.Save(_env.WebRootPath, "uploads/books", book.HoverPosterImage)
            };

            book.BookImages.Add(hoverPosterImage);
          

            foreach (var item in book.Images)
            {
                BookImage bookImage = new BookImage()
                {
                    PosterStatus = null,
                    Image = FileManager.Save(_env.WebRootPath, "uploads/books", item)
                };

                book.BookImages.Add(bookImage);
            }

            if (book.TagIds != null)
            {
                foreach (var item in book.TagIds)
                {
                    BookTag bookTag = new BookTag()
                    {
                        TagId = item
                    };

                    book.BookTags.Add(bookTag);
                }
            }

            #endregion


            _context.Books.Add(book);

            try
            {
                _context.SaveChanges();
            }
            catch (Exception)
            {
                foreach (var item in book.BookImages)
                {
                    FileManager.Delete(_env.WebRootPath, "uploads/books", item.Image);
                }
            }
           

            return RedirectToAction("index");
        }

        public IActionResult Update(int id)
        {
            Book book = _context.Books.Include(x=>x.BookImages).Include(x=>x.BookTags).FirstOrDefault(x => x.Id == id);

            
            book.TagIds = book.BookTags.Select(x => x.TagId).ToList();

            if (book == null)
            {
                Response.StatusCode = 404;
                return View("BookNotFound", id);
            }

            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genres = _context.Genres.ToList();
            ViewBag.Publishers = _context.Publishers.ToList();
            ViewBag.Tags = _context.Tags.ToList();

            return View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Book book)
        {
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genres = _context.Genres.ToList();
            ViewBag.Publishers = _context.Publishers.ToList();
            ViewBag.Tags = _context.Tags.ToList();

            if (!ModelState.IsValid) return View();

            Book existBook = _context.Books.Include(x=>x.BookImages).Include(x=>x.BookTags).FirstOrDefault(x => x.Id == book.Id);

            if (existBook == null)
            {
                Response.StatusCode = 404;
                return View("BookNotFound", book.Id);
            }

            #region AnyCheck

            if (!_context.Categories.Any(x => x.Id == book.CategoryId))
            {
                ModelState.AddModelError("CategoryId", "Have not Category like this name!");
                return View(existBook);
            }

            if (!_context.Authors.Any(x => x.Id == book.AuthorId))
            {
                ModelState.AddModelError("AuthorId", "Have not Author like this fullname!");
                return View(existBook);
            }

            if (!_context.Genres.Any(x => x.Id == book.GenreId))
            {
                ModelState.AddModelError("GenreId", "Have not Genre like this name!");
                return View(existBook);
            }

            if (!_context.Publishers.Any(x => x.Id == book.PublisherId))
            {
                ModelState.AddModelError("PublisherId", "Have not Publisher like this name!");
                return View(existBook);
            }

            if (_context.Books.Any(x => x.Code == book.Code)&&book.Code!=existBook.Code)
            {
                ModelState.AddModelError("Code", "Code is required!");
                return View(existBook);
            }

            #endregion

            #region ImageChack

            if (book.PosterImage != null)
            {
                if (book.PosterImage.ContentType != "image/jpeg" && book.PosterImage.ContentType != "image/png")
                {
                    ModelState.AddModelError("PosterImage", "You can choose file only .jpg, .jpeg or .png format!");
                    return View(existBook);
                }

                if (book.PosterImage.Length > 5242880)
                {
                    ModelState.AddModelError("PosterImage", "You can choose file only maximum 5Mb !");
                    return View(existBook);
                }
            }

            if (book.HoverPosterImage != null)
            {
                if (book.HoverPosterImage.ContentType != "image/jpeg" && book.HoverPosterImage.ContentType != "image/png")
                {
                    ModelState.AddModelError("HoverPosterImage", "You can choose file only .jpg, .jpeg or .png format!");
                    return View(existBook);
                }

                if (book.HoverPosterImage.Length > 5242880)
                {
                    ModelState.AddModelError("HoverPosterImage", "You can choose file only maximum 5Mb !");
                    return View(existBook);
                }
            }

            

            if (book.Images != null)
            {
                foreach (var item in book.Images)
                {
                    if (item.ContentType != "image/jpeg" && item.ContentType != "image/png")
                    {
                        ModelState.AddModelError("Images", "You can choose file only .jpg, .jpeg or .png format!");
                        return View(existBook);
                    }

                    if (item.Length > 5242880)
                    {
                        ModelState.AddModelError("Images", "You can choose file only maximum 5Mb !");
                        return View(existBook);
                    }
                }
            }

            #endregion


            existBook.Name = book.Name;
            existBook.CategoryId = book.CategoryId;
            existBook.AuthorId = book.AuthorId;
            existBook.GenreId = book.GenreId;
            existBook.PublisherId = book.PublisherId;
            existBook.ProducingPrice = book.ProducingPrice;
            existBook.Price = book.Price;
            existBook.Code = book.Code;
            existBook.PublicationDate = book.PublicationDate;
            existBook.IsAvailable = book.IsAvailable;
            existBook.Desc = book.Desc;

            List<string> newFileNames = new List<string>();

            if (book.PosterImage != null)
            {
                string filename = FileManager.Save(_env.WebRootPath, "uploads/books", book.PosterImage);
                BookImage oldPoster = existBook.BookImages.FirstOrDefault(x => x.PosterStatus == true);

                if (oldPoster != null)
                {
                    FileManager.Delete(_env.WebRootPath, "uploads/books", oldPoster.Image);
                    oldPoster.Image = filename;
                }
                else
                {
                    BookImage bookImage = new BookImage()
                    {
                        PosterStatus = true,
                        Image = filename
                    };
                    existBook.BookImages.Add(bookImage);
                }

                newFileNames.Add(filename);

            }

            if (book.HoverPosterImage != null)
            {
                string filename = FileManager.Save(_env.WebRootPath, "uploads/books", book.HoverPosterImage);
                BookImage oldPoster = existBook.BookImages.FirstOrDefault(x => x.PosterStatus == false);

                if (oldPoster != null)
                {
                    FileManager.Delete(_env.WebRootPath, "uploads/books", oldPoster.Image);
                    oldPoster.Image = filename;
                }
                else
                {
                    BookImage bookImage = new BookImage()
                    {
                        PosterStatus = false,
                        Image = filename
                    };
                    existBook.BookImages.Add(bookImage);
                }
                newFileNames.Add(filename);
            }

            if (book.ImageIds != null)
            {
                foreach (var item in existBook.BookImages.FindAll(x => x.PosterStatus == null))
                {
                    if (!book.ImageIds.Any(x => x == item.Id))
                    {
                        FileManager.Delete(_env.WebRootPath, "uploads/books", item.Image);

                        _context.BookImages.Remove(item);
                    }
                }
            }

            if (book.Images != null)
            {
                foreach (var item in book.Images)
                {
                    string newFileName = FileManager.Save(_env.WebRootPath, "uploads/books", item);

                    BookImage bookImage = new BookImage()
                    {
                        Image = newFileName
                    };
                    existBook.BookImages.Add(bookImage);

                    newFileNames.Add(newFileName);
                }
            }

            if (book.TagIds != null)
            {
                List<BookTag> bookTags = new List<BookTag>();
                foreach (var item in book.TagIds)
                {
                    BookTag bookTag = new BookTag()
                    {
                        TagId = item
                    };
                    bookTags.Add(bookTag);
                }
                existBook.BookTags = bookTags;
            }
            else
            {
                existBook.BookTags.Clear();
            }

            try
            {
                _context.SaveChanges();
            }
            catch (Exception)
            {
                foreach (var item in newFileNames)
                {
                    FileManager.Delete(_env.WebRootPath, "uploads/books", item);
                }
                
                return View("BookNotFound", book.Id);
            }

            return RedirectToAction("index");
        }

        public IActionResult Delete(int id)
        {

            Book existBook = _context.Books.Include(x=>x.BookImages).FirstOrDefault(x => x.Id == id);

            if (existBook == null)
            {
                Response.StatusCode = 404;
                return View("BookNotFound", id);
            }

            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genres = _context.Genres.ToList();
            ViewBag.Publishers = _context.Publishers.ToList();

            return View(existBook);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Book book)
        {
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genres = _context.Genres.ToList();
            ViewBag.Publishers = _context.Publishers.ToList();


            Book existBook = _context.Books.Include(x=>x.BookImages).FirstOrDefault(x => x.Id == book.Id);

            if (existBook == null)
            {
                Response.StatusCode = 404;
                return View("BookNotFound", book.Id);
            }

            if (existBook.BookImages != null)
            {
                foreach (var item in existBook.BookImages)
                {
                    FileManager.Delete(_env.WebRootPath, "uploads/books", item.Image);
                }
            }

            _context.Books.Remove(existBook);
            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Comment(int bookId, int page=1)
        { 
            if (!_context.Books.Any(x=>x.Id == bookId)) return View("BookNotFound", bookId);

            ViewBag.BookId = bookId;

            var query = _context.Comments.Include(x=>x.AppUser).Where(x => x.BookId == bookId);
            
            int totalComments = query.Count();

            ViewBag.SelectedPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalComments / 2d);
            ViewBag.TotalComments = totalComments;

            var comments = query.Skip((page - 1) * 2).Take(2).ToList();

            return View(comments);
        }

        public IActionResult CommentAccept(int bookId, int id)
        {
            Comment comment = _context.Comments.FirstOrDefault(x => x.Id == id && x.BookId == bookId);

            if(comment == null) return View("BookNotFound", bookId);

            ViewBag.BookId = bookId;

            comment.Status = true;

            _context.SaveChanges();

            Book book = _context.Books.Include(x => x.Comments).FirstOrDefault(x => x.Id == comment.BookId);

            var acceptedComments = book.Comments.Where(x => x.Status == true);

            book.Rate = acceptedComments.Count()==0d?0:acceptedComments.Average(x => (double)x.Rate);

            _context.SaveChanges();

            return RedirectToAction("comment", new { bookId = comment.BookId });
        }

        public IActionResult CommentReject(int bookId, int id)
        {
            Comment comment = _context.Comments.FirstOrDefault(x => x.Id == id && x.BookId == bookId);

            if (comment == null) return View("BookNotFound", bookId);

            ViewBag.BookId = bookId;

            comment.Status = false;
            _context.SaveChanges();

            Book book = _context.Books.Include(x => x.Comments).FirstOrDefault(x => x.Id == comment.BookId);

            var acceptedComments = book.Comments.Where(x => x.Status == true);

            book.Rate = acceptedComments.Count() == 0d ? 0 : acceptedComments.Average(x => (double)x.Rate);

            _context.SaveChanges();

            return RedirectToAction("comment", new { bookId = comment.BookId });
        }
    }
}
