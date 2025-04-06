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