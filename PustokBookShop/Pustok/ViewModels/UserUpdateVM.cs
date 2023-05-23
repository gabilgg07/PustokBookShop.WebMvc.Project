using Pustok.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.ViewModels
{
    public class UserUpdateVM
    {
        [StringLength(maximumLength: 50)]
        public string FullName { get; set; }

        [Required]
        [StringLength(maximumLength: 25)]
        public string UserName { get; set; }

        [Required]
        [StringLength(maximumLength: 125)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [StringLength(maximumLength: 25)]
        public string Country { get; set; }

        [StringLength(maximumLength: 25)]
        public string City { get; set; }

        [StringLength(maximumLength: 25)]
        public string State { get; set; }

        [StringLength(maximumLength: 125)]
        public string Address { get; set; }

        public List<Order> Orders { get; set; }
    }
}
