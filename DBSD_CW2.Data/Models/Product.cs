using System;
using System.ComponentModel.DataAnnotations;

namespace DBSD_CW2.Data.Models
{
    public class Product
    {
        public int ProductId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int StockQuantity { get; set; }

        public byte[] ImageData { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public bool IsActive { get; set; }

        public int? CategoryId { get; set; }

        [Required]
        public DateTime LastModifiedDate { get; set; }

        // Navigation property
        public virtual Category Category { get; set; }
    }
} 