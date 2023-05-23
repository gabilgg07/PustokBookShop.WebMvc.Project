using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Models
{
    public class Feature
    {
        public int Id { get; set; }
        [StringLength(maximumLength:100)]
        public string Icon { get; set; }
       
        [Required]
        [StringLength(maximumLength: 50)]
        public string Title { get; set; }
        
        [StringLength(maximumLength: 50)]
        public string Subtitle { get; set; }

        [Required]
        public int Order { get; set; }
    }
}
