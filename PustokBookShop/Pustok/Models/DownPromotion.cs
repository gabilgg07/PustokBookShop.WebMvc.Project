using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Models
{
    public class DownPromotion
    {
        public int Id { get; set; }

        [Required]
        [StringLength(maximumLength: 100)]
        public string Image { get; set; }

        [StringLength(maximumLength: 250)]
        public string PriceText { get; set; }

        [Required]
        [StringLength(maximumLength: 100)]
        public string Title { get; set; }

        [StringLength(maximumLength: 200)]
        public string Subtitle { get; set; }

        [Required]
        [StringLength(maximumLength: 100)]
        public string BtnText { get; set; }

        [Required]
        [StringLength(maximumLength: 250)]
        public string RedirectUrl { get; set; }
    }
}
