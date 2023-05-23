using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Models
{
    public class CampaignBook
    {
        public int Id { get; set; }

        public int CampaignId { get; set; }

        public Campaign Campaign { get; set; }

        public int BookId { get; set; }

        public Book Book { get; set; }
    }
}
