using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Models
{
    public class Partner
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Image { get; set; }
    }
}
