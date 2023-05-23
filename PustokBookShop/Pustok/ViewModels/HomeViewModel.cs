using Pustok.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.ViewModels
{
    public class HomeViewModel
    {
        public List<Slider> Sliders { get; set; }
        public List<Feature> Features { get; set; }
        public List<UpPromotion> UpPromotions { get; set; }
        public List<DownPromotion> DownPromotions { get; set; }

        public List<Book> FeaturedBooks { get; set; }
        public List<Book> NewArrivalBooks { get; set; }
        public List<Book> MostViewBooks { get; set; }

        public List<Book> ChildrenBooks { get; set; }

        public List<Book> ArtBooks { get; set; }

        public List<CampaignBook> BooksOfCampain { get; set; }

    }
}
