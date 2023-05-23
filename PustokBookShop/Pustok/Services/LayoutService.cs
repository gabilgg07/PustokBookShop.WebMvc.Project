using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pustok.DAL;
using Pustok.Models;
using Pustok.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Services
{
    public class LayoutService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;

        public LayoutService(AppDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public Setting GetSetting()
        {
            return _context.Settings.FirstOrDefault();
        }

        public List<Partner> GetPartners()
        {
            return _context.Partners.ToList();
        }

        public List<Category> GetCategories()
        {
            return _context.Categories.ToList();
        }

        public BasketVM GetBasket()
        {
            
            var basket = _httpContextAccessor.HttpContext.Request.Cookies["Basket"];

            BasketVM basketData = new BasketVM()
            {
                BasketItemVMs = new List<BasketItemVM>(),
                TotalPrice=0
            };

            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated && _userManager.Users.Any(x => x.UserName == _httpContextAccessor.HttpContext.User.Identity.Name && x.IsAdmin == false))
            {
                var basketItems = _context.BasketItems.Include(x => x.AppUser).Include(x => x.Book).ThenInclude(x => x.BookImages).Where(x => x.AppUser.UserName == _httpContextAccessor.HttpContext.User.Identity.Name);

                foreach (var item in basketItems)
                {
                    BasketItemVM basketItemVM = new BasketItemVM
                    {
                        Book = item.Book,
                        Count = item.Count
                    };

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
            else
            {
                if (basket != null)
                {
                    List<BasketCookieItemVM> basketCookies = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(basket);



                    foreach (var item in basketCookies)
                    {

                        BasketItemVM basketItem = new BasketItemVM()
                        {
                            Book = _context.Books.Include(x => x.BookImages).Include(x=>x.CampaignBooks).ThenInclude(x=>x.Campaign).FirstOrDefault(x => x.Id == item.Id),
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
            }

           
            return basketData;
        }
    }
}
