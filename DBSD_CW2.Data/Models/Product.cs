using System;
using System.ComponentModel.DataAnnotations;

namespace DBSD_CW2.Data.Models
{
    public class Product
    {
        public int ProductId { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }
        
        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [Phone]
        [StringLength(20)]
        public string Phone { get; set; }
        
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }
        
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }

        public string Description { get; set; }

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