using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Models
{
    public class Campaign
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [Column(TypeName = "money")]
        public double? DiscountPercent { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? ExpireDate { get; set; }

        public List<CampaignBook> CampaignBooks { get; set; }
    }
}
