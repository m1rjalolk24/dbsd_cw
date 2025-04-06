using DBSD_CW2.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DBSD_CW2.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

               // Add Categories
            var categories = new Category[]
            {
                new Category
                {
                    Name = "Electronics",
                    Description = "Electronic devices and accessories",
                    CreatedDate = DateTime.UtcNow
                },
                new Category
                {
                    Name = "Books",
                    Description = "Books and publications",
                    CreatedDate = DateTime.UtcNow
                },
                new Category
                {
                    Name = "Clothing",
                    Description = "Apparel and fashion items",
                    CreatedDate = DateTime.UtcNow
                }
            };

            // Look for any products
            if (!context.Categories.Any())
            {

            foreach (var c in categories)
            {
                context.Categories.Add(c);
            }
            context.SaveChanges();
            }

            

            // Add Products
            var products = new Product[]
            {
                new Product
                {
                    FirstName = "Laptop",
                    LastName = "Pro",
                    Phone = "+1 (555) 123-4567",
                    Email = "laptop.pro@example.com",
                    DateOfBirth = new DateTime(2023, 1, 15),
                    Description = "High-performance laptop with latest specifications",
                    Price = 999.99m,
                    StockQuantity = 50,
                    CategoryId = categories[0].CategoryId,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    LastModifiedDate = DateTime.UtcNow
                },
                new Product
                {
                    FirstName = "Smartphone",
                    LastName = "X",
                    Phone = "+1 (555) 234-5678",
                    Email = "smartphone.x@example.com",
                    DateOfBirth = new DateTime(2022, 6, 10),
                    Description = "Latest model smartphone with advanced features",
                    Price = 699.99m,
                    StockQuantity = 100,
                    CategoryId = categories[0].CategoryId,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    LastModifiedDate = DateTime.UtcNow
                },
                new Product
                {
                    FirstName = "Programming",
                    LastName = "Guide",
                    Phone = "+1 (555) 345-6789",
                    Email = "programming.guide@example.com",
                    DateOfBirth = new DateTime(2021, 3, 22),
                    Description = "Comprehensive programming guide for beginners",
                    Price = 49.99m,
                    StockQuantity = 75,
                    CategoryId = categories[1].CategoryId,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    LastModifiedDate = DateTime.UtcNow
                },
                new Product
                {
                    FirstName = "Fantasy",
                    LastName = "Novel",
                    Phone = "+1 (555) 456-7890",
                    Email = "fantasy.novel@example.com",
                    DateOfBirth = new DateTime(2020, 9, 5),
                    Description = "Bestselling fiction novel",
                    Price = 24.99m,
                    StockQuantity = 200,
                    CategoryId = categories[1].CategoryId,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    LastModifiedDate = DateTime.UtcNow
                },
                new Product
                {
                    FirstName = "Cotton",
                    LastName = "T-Shirt",
                    Phone = "+1 (555) 567-8901",
                    Email = "cotton.tshirt@example.com",
                    DateOfBirth = new DateTime(2023, 4, 18),
                    Description = "Cotton t-shirt with modern design",
                    Price = 19.99m,
                    StockQuantity = 150,
                    CategoryId = categories[2].CategoryId,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    LastModifiedDate = DateTime.UtcNow
                },
                new Product
                {
                    FirstName = "Denim",
                    LastName = "Jeans",
                    Phone = "+1 (555) 678-9012",
                    Email = "denim.jeans@example.com",
                    DateOfBirth = new DateTime(2022, 11, 30),
                    Description = "Classic denim jeans",
                    Price = 59.99m,
                    StockQuantity = 100,
                    CategoryId = categories[2].CategoryId,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    LastModifiedDate = DateTime.UtcNow
                }
            };

            foreach (var p in products)
            {
                context.Products.Add(p);
            }
            context.SaveChanges();
        }
    }
} 