using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Column(TypeName = "Money")]
        public double ProducingPrice { get; set; }

        [Column(TypeName = "Money")]
        public double Price { get; set; }

        [Column(TypeName = "Money")]
        public double? DiscountedPrice { get; set; }

        [MaxLength(20)]
        public string Code { get; set; }

        public bool IsAvailable { get; set; }

        public bool? IsFeatured { get; set; }

        [Column(TypeName = "datetime2(2)")]
        public DateTime? PublicationDate { get; set; }

        [MaxLength(4500)]
        public string Desc { get; set; }

        [Column(TypeName = "decimal(4,2)")]
        public double? Rate { get; set; }

        [NotMapped]
        public IFormFile PosterImage { get; set; }

        [NotMapped]
        public IFormFile HoverPosterImage { get; set; }

        [NotMapped]
        public List<IFormFile> Images { get; set; }

        [NotMapped]
        public List<int> ImageIds { get; set; }

        [NotMapped]
        public List<int> TagIds { get; set; }

        public int? AuthorId { get; set; }

        public Author Author { get; set; }

        public int? GenreId { get; set; }

        public Genre Genre { get; set; }

        public int? PublisherId { get; set; }

        public Publisher Publisher { get; set; }

        public int? CategoryId { get; set; }

        public Category Category { get; set; }

        public List<BookImage> BookImages { get; set; }

        public List<BookTag> BookTags { get; set; }

        public List<CampaignBook> CampaignBooks { get; set; }

        public List<Comment> Comments { get; set; }

        public List<BasketItem> BasketItems { get; set; }

        public List<OrderItem> OrderItems { get; set; }

    }
}
