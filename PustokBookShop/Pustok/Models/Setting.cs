using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Models
{
    public class Setting
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string HeaderLogo { get; set; }

        [NotMapped]
        public IFormFile HeaderImage { get; set; }

        [MaxLength(100)]
        public string FooterLogo { get; set; }

        [NotMapped]
        public IFormFile FooterImage { get; set; }


        [Required]
        [MaxLength(100)]
        public string Phone1 { get; set; }

        [MaxLength(100)]
        public string Phone2 { get; set; }

        [Required]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        [MaxLength(100)]
        public string Address { get; set; }
    }
}
