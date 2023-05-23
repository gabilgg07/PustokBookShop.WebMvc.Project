using Pustok.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.ViewModels
{
    public class OrderCreateVM
    {
        [Required]
        [StringLength(maximumLength: 25)]
        public string Country { get; set; }

        [Required]
        [StringLength(maximumLength: 25)]
        public string City { get; set; }

        [Required]
        [StringLength(maximumLength: 25)]
        public string State { get; set; }

        [Required]
        [StringLength(maximumLength: 125)]
        public string Address { get; set; }

        [StringLength(maximumLength:250)]
        public string Note { get; set; }

        public List<BasketItem> BasketItems { get; set; }
    }
}
