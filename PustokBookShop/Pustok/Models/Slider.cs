using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Models
{
    public class Slider
    {
        public int Id { get; set; }

        [StringLength(maximumLength:100)]
        public string Title { get; set; }

        [StringLength(maximumLength: 200)]
        public string Subtitle { get; set; }
        
        [StringLength(maximumLength: 200)]
        public string Image { get; set; }

        [NotMapped]
        public IFormFile ImageFile { get; set; }

        [StringLength(maximumLength: 250)]
        public string RedirectUrl { get; set; }

        [Required]
        public int Order { get; set; }
  
        [StringLength(maximumLength: 25)]
        public string BtnText { get; set; }
    }
}
