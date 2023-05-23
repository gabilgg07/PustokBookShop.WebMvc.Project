using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Models
{
    public class AppUser:IdentityUser
    {
        [StringLength(maximumLength: 50)]
        public string FullName { get; set; }

        public bool IsAdmin { get; set; }

        [StringLength(maximumLength:100)]
        public string Image { get; set; }

        [NotMapped]
        public IFormFile ImageFile { get; set; }

        [StringLength(maximumLength: 25)]
        public string Country { get; set; }

        [StringLength(maximumLength: 25)]
        public string City { get; set; }

        [StringLength(maximumLength: 25)]
        public string State { get; set; }

        [StringLength(maximumLength: 125)]
        public string Address { get; set; }

        public string ConnectionId { get; set; }

        public List<BasketItem> BasketItems { get; set; }
        public List<Comment> Comments { get; set; }
        public List<Order> Orders { get; set; }
    }
}
