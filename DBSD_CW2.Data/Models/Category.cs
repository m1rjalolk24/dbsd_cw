using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        [Display(Name = "Price Per Month")]
        public decimal PricePerMonth { get; set; }

        [Required]
        [Display(Name = "Duration (Months)")]
        public int Duration { get; set; }

        // Navigation property
        public virtual ICollection<Product> Products { get; set; }
    }
} 