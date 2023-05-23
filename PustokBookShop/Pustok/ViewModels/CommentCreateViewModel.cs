using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.ViewModels
{
    public class CommentCreateViewModel
    {
        public int BookId { get; set; }

        [StringLength(maximumLength: 250)]
        public string Text { get; set; }
        [Required]
        [Range(1, 5)]
        public int Rate { get; set; }
    }
}
