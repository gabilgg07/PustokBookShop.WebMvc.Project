using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Models
{
    public class Author
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string FullName { get; set; }


        [MaxLength(1500)]
        public string BioDesc { get; set; }

        [MaxLength(200)]
        public string Image { get; set; }

        [NotMapped]
        public IFormFile ImageFile { get; set; }

        public List<Book> Books { get; set; }
    }
}
