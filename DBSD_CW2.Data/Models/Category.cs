using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DBSD_CW2.Data.Models
{
    public class Category
    {
        public int CategoryId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        // Navigation property
        public virtual ICollection<Product> Products { get; set; }
    }
} 