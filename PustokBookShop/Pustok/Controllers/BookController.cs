using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pustok.DAL;
using Pustok.Models;
using Pustok.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Controllers
{
    public class BookController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public BookController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index(int? categoryId,int page=1)
        {
            var query = _context.Books.AsQueryable();

            if (categoryId != null)
            {
                query = query.Where(x => x.CategoryId == categoryId);
            }

            var totalPages = query.Count()/3d;
            ViewBag.TotalPages = Math.Ceiling(totalPages);
            ViewBag.SelectedPage = page;
            ViewBag.CategoryId = categoryId;
            List<Book> books = query
                .Include(x => x.Author)
                .Include(x=>x.BookImages)
                .Include(x=>x.Comments)
                .Skip((page-1)*3)
                .Take(3)
                .ToList();
            return View(books);
        }

        public IActionResult Detail(int id)
        {
            Book book = _context.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .Include(b => b.BookImages)
                .Include(b => b.Comments)
                .ThenInclude(c => c.AppUser)
                .Include(b => b.BookTags)
                .ThenInclude(bt => bt.Tag)
                .Include(b => b.CampaignBooks)
                .ThenInclude(cb => cb.Campaign)
                .FirstOrDefault(b => b.Id == id);

            if (book == null) return NotFound();

            List<Book> books = _context.Books
                .Include(b => b.Author)
                .Include(b => b.BookImages)
                .Include(b => b.Comments)
                .ThenInclude(c => c.AppUser)
                .Include(b => b.CampaignBooks)
                .ThenInclude(cb => cb.Campaign)
                .ToList().GetRange(_context.Books.ToList().Count - 5, 5);

            books.Add(book);

            return View(books);
        }

        public IActionResult GetDetaileBook(int id)
        {
            Book book = _context.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .Include(b => b.BookImages)
                .Include(b => b.Comments)
                .ThenInclude(c => c.AppUser)
                .Include(b => b.BookTags)
                .ThenInclude(bt => bt.Tag)
                .Include(b => b.CampaignBooks)
                .ThenInclude(cb => cb.Campaign)
                .FirstOrDefault(b => b.Id == id);

            return PartialView("_BookDetail", book);
        }
      
        public IActionResult AddToBasket(int id)
        {
            Book book = _context.Books.Include(x => x.BasketItems).FirstOrDefault(b => b.Id == id);

            if (book == null) return RedirectToAction("index", "NotFound");
           

            BasketVM basketData = new BasketVM()
            {
                BasketItemVMs = new List<BasketItemVM>(),
                TotalPrice = 0
            };

            if (User.Identity.IsAuthenticated && _userManager.Users.Any(x => x.UserName == User.Identity.Name && x.IsAdmin == false))
            {
                AppUser user = _userManager.FindByNameAsync(User.Identity.Name).Result;

                BasketItem basketItem = book.BasketItems.FirstOrDefault(x => x.AppUserId == user.Id);
                if (basketItem != null)
                {
                    basketItem.Count++;
                }
                else
                {
                    basketItem = new BasketItem
                    {
                        AppUserId = user.Id,
                        Count = 1,
                        BookId = id
                    };

                    book.BasketItems.Add(basketItem);
                }

                _context.SaveChanges();

                foreach (var item in _context.BasketItems.Include(x => x.AppUser).Include(x => x.Book).ThenInclude(x => x.BookImages).Where(x=>x.AppUser.UserName == User.Identity.Name).ToList())
                {
                    BasketItemVM basketItemVM = new BasketItemVM()
                    {
                        Book = _context.Books.Include(x => x.BookImages).FirstOrDefault(x => x.Id == item.BookId),
                        Count = item.Count
                    };

                    if (basketItemVM.Book != null)
                    {
                        if (basketItemVM.Book.DiscountedPrice == null)
                        {
                            basketData.TotalPrice += basketItemVM.Book.Price * basketItemVM.Count;
                        }
                        else
                        {
                            basketData.TotalPrice += (double)basketItemVM.Book.DiscountedPrice * basketItemVM.Count;
                        }
                        basketData.BasketItemVMs.Add(basketItemVM);
                        basketData.Count++;
                    }
                }
            }
            else
            {
                var basket = HttpContext.Request.Cookies["Basket"];

                List<BasketCookieItemVM> basketItems = new List<BasketCookieItemVM>();

                if (basket == null)
                {
                    basketItems = new List<BasketCookieItemVM>() 
                    {
                        new BasketCookieItemVM() 
                        {
                            Id = book.Id,
                            Count = 1
                         }
                     };
                }
                else
                {
                    basketItems = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(basket);
                    BasketCookieItemVM basketItem = basketItems.FirstOrDefault(x => x.Id == book.Id);
                    if (basketItem == null)
                    {
                        basketItem = new BasketCookieItemVM()
                        {
                            Id = book.Id,
                            Count = 1
                        };
                        basketItems.Add(basketItem);
                    }
                    else
                    {
                        basketItem.Count++;
                    }
                }

                var basketItemsStr = JsonConvert.SerializeObject(basketItems);

                HttpContext.Response.Cookies.Append("Basket", basketItemsStr);

                foreach (var item in basketItems)
                {

                    BasketItemVM basketItem = new BasketItemVM()
                    {
                        Book = _context.Books.Include(x => x.BookImages).FirstOrDefault(x => x.Id == item.Id),
                        Count = item.Count
                    };

                    if (basketItem.Book != null)
                    {
                        if (basketItem.Book.DiscountedPrice == null)
                        {
                            basketData.TotalPrice += basketItem.Book.Price * basketItem.Count;
                        }
                        else
                        {
                            basketData.TotalPrice += (double)basketItem.Book.DiscountedPrice * basketItem.Count;
                        }
                        basketData.BasketItemVMs.Add(basketItem);
                        basketData.Count++;
                    }
                }
            }
            return PartialView("_BasketPartial", basketData);
        }

        public IActionResult DeleteFromBasket(int id)
        {

            Book book = _context.Books.Include(x => x.BasketItems).FirstOrDefault(b => b.Id == id);

            if (book == null) return RedirectToAction("index", "NotFound");

            BasketVM basketData = new BasketVM()
            {
                BasketItemVMs = new List<BasketItemVM>(),
                TotalPrice = 0
            };

            if (User.Identity.IsAuthenticated && _userManager.Users.Any(x => x.UserName == User.Identity.Name && x.IsAdmin == false))
            {
                AppUser user = _userManager.FindByNameAsync(User.Identity.Name).Result;

                BasketItem basketItem = book.BasketItems.FirstOrDefault(x => x.AppUserId == user.Id);
                if (basketItem.Count>1)
                {
                    basketItem.Count--;
                }
                else
                {
                    book.BasketItems.RemoveAll(x => x.AppUserId == user.Id);
                }

                _context.SaveChanges();

                foreach (var item in _context.BasketItems.Include(x => x.AppUser).Include(x => x.Book).ThenInclude(x => x.BookImages).Where(x => x.AppUser.UserName == User.Identity.Name).ToList())
                {
                    BasketItemVM basketItemVM = new BasketItemVM()
                    {
                        Book = _context.Books.Include(x => x.BookImages).FirstOrDefault(x => x.Id == item.BookId),
                        Count = item.Count
                    };

                    if (basketItemVM.Book != null)
                    {
                        if (basketItemVM.Book.DiscountedPrice == null)
                        {
                            basketData.TotalPrice += basketItemVM.Book.Price * basketItemVM.Count;
                        }
                        else
                        {
                            basketData.TotalPrice += (double)basketItemVM.Book.DiscountedPrice * basketItemVM.Count;
                        }
                        basketData.BasketItemVMs.Add(basketItemVM);
                        basketData.Count++;
                    }
                }
            }
            else
            {
                var basket = HttpContext.Request.Cookies["Basket"];
                List<BasketCookieItemVM> basketItems;


                basketItems = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(basket);

                BasketCookieItemVM basketItemVM = basketItems.FirstOrDefault(x => x.Id == id);

                if (basketItemVM.Count > 1)
                {
                    basketItems.FirstOrDefault(x => x.Id == id).Count--;
                }
                else
                {
                    basketItems.RemoveAll(x => x.Id == id);
                }

                var basketItemsStr = JsonConvert.SerializeObject(basketItems);
                HttpContext.Response.Cookies.Append("Basket", basketItemsStr);



                foreach (var item in basketItems)
                {
                    BasketItemVM basketItem = new BasketItemVM()
                    {
                        Book = _context.Books.Include(x => x.BookImages).FirstOrDefault(x => x.Id == item.Id),
                        Count = item.Count
                    };

                    if (basketItem.Book != null)
                    {
                        if (basketItem.Book.DiscountedPrice == null)
                        {
                            basketData.TotalPrice += basketItem.Book.Price * basketItem.Count;
                        }
                        else
                        {
                            basketData.TotalPrice += (double)basketItem.Book.DiscountedPrice * basketItem.Count;
                        }
                        basketData.BasketItemVMs.Add(basketItem);
                        basketData.Count++;
                    }

                }
            }
            
            return PartialView("_BasketPartial", basketData);
        }


        [Authorize(Roles = "Member")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Addcomment(CommentCreateViewModel commentVM)
        {
            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (!ModelState.IsValid)
                return RedirectToAction("detail", "book", new { id = commentVM.BookId });

            Book book = _context.Books.Include(x => x.Comments).FirstOrDefault(x => x.Id == commentVM.BookId);
            if (book == null)
                return RedirectToAction("index", "NotFound");

            if (_context.Comments.Any(x => x.BookId == commentVM.BookId && x.Status == true && x.AppUserId == user.Id))
            {
                TempData["Error"] = "You can't again give rate to this book!"; 
                return RedirectToAction("detail", "book", new { id = commentVM.BookId });
            }
               


            Comment comment = new Comment
            {
                BookId = commentVM.BookId,
                AppUserId = user.Id,
                Text = commentVM.Text,
                Rate = commentVM.Rate,
                CreatedAt = DateTime.UtcNow
            };
            book.Comments.Add(comment);

            _context.SaveChanges();
            return RedirectToAction("detail", "book", new { id = commentVM.BookId });
        }

        public IActionResult LoadComments(int bookId, int page = 1)
        {
            if (!_context.Books.Any(x => x.Id == bookId)) return Json(new { status = 404 });


            var comments = _context.Comments.OrderByDescending(x => x.CreatedAt).Include(x => x.AppUser).Where(x => x.BookId == bookId && x.Status == true).Take(2 * page).ToList();

            return PartialView("_BookComments", comments);
        }
    }
}
