using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pustok.DAL;
using Pustok.Models;
using Pustok.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            HomeViewModel homeVM = new HomeViewModel
            {
                Sliders = _context.Sliders.OrderBy(x=>x.Order).ToList(),
                Features = _context.Features.OrderBy(x=>x.Order).ToList(),
                UpPromotions = _context.UpPromotions.ToList(),
                DownPromotions = _context.DownPromotions.ToList(),
                ChildrenBooks = _context.Books.Where(b=>b.CategoryId==2).Take(6)
                   .Include(b=>b.Author)
                   .Include(b=>b.BookImages).ToList(),
                ArtBooks = _context.Books.Where(b=>b.CategoryId==3).Take(7)
                    .Include(b=>b.Author)
                    .Include(b=>b.BookImages).ToList(),
                BooksOfCampain = _context.CampaignBooks
                    .Include(cb=>cb.Book).ThenInclude(b => b.Author)
                    .Include(cb => cb.Book).ThenInclude(b => b.BookImages)
                    .Include(cb=>cb.Campaign).Where(cb=>cb.CampaignId==1).Take(7).ToList(),
                FeaturedBooks = _context.Books.Where(b=>b.IsFeatured==true)
                .Include(b => b.Author)
                .Include(b => b.BookImages)
                .Include(b=>b.CampaignBooks)
                .ThenInclude(cb=>cb.Campaign).Take(8).ToList(),
                NewArrivalBooks = _context.Books.Where(b=>b.IsFeatured==false)
                .Include(b => b.Author)
                .Include(b => b.BookImages)
                .Include(b => b.CampaignBooks)
                .ThenInclude(cb => cb.Campaign).Take(8).ToList(),
                MostViewBooks = _context.Books.Where(b=>b.IsFeatured==null)
                .Include(b => b.Author)
                .Include(b => b.BookImages)
                .Include(b => b.CampaignBooks)
                .ThenInclude(cb => cb.Campaign).Take(8).ToList()
            };

            return View(homeVM);
        }

    }
}
