using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int BookId { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int Count { get; set; }

        public Book Book { get; set; }
        public Order Order { get; set; }
    }
}
