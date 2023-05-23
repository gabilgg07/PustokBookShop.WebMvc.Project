using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(35,ErrorMessage = "Uzunluq 35-den boyuk ola bilmez!!!")]
        public string Name { get; set; }

        public List<Book> Books { get; set; }
    }
}
