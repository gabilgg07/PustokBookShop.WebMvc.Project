using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Models
{
    public class UpPromotion
    {
        public int Id { get; set; }
        [Required]
        [StringLength(maximumLength:100)]
        public string Image { get; set; }
        [Required]
        [StringLength(maximumLength:250)]
        public string RedirectUrl { get; set; }
    }
}
