using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DBSD_CW2.Data;
using DBSD_CW2.Data.Models;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace DBSD_CW2.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["PriceSortParm"] = sortOrder == "Price" ? "price_desc" : "Price";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var products = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.FirstName.Contains(searchString)
                                       || p.LastName.Contains(searchString)
                                       || p.Phone.Contains(searchString)
                                       || p.Email.Contains(searchString)
                                       || p.Description.Contains(searchString)
                                       || p.Category.Name.Contains(searchString));
            }

            products = sortOrder switch
            {
                "name_desc" => products.OrderByDescending(p => p.FirstName),
                "Price" => products.OrderBy(p => p.Price),
                "price_desc" => products.OrderByDescending(p => p.Price),
                "Date" => products.OrderBy(p => p.CreatedDate),
                "date_desc" => products.OrderByDescending(p => p.CreatedDate),
                _ => products.OrderBy(p => p.FirstName)
            };

            int pageSize = 10;
            return View(await PaginatedList<Product>.CreateAsync(products, pageNumber ?? 1, pageSize));
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_context.Categories, "CategoryId", "Name");
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Phone,Email,DateOfBirth,Description,Price,StockQuantity,IsActive,CategoryId")] Product product, IFormFile imageFile)
        {
                if (imageFile != null && imageFile.Length > 0)
                {
                    using (var stream = new MemoryStream())
                    {
                        await imageFile.CopyToAsync(stream);
                        product.ImageData = stream.ToArray();
                    }
                }
                else
                {
                    ModelState.AddModelError("ImageData", "Product image is required.");
                    ViewData["Categories"] = new SelectList(_context.Categories, "CategoryId", "Name", product.CategoryId);
                    return View(product);
                }

                product.CreatedDate = DateTime.UtcNow;
                product.LastModifiedDate = DateTime.UtcNow;
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            
            ViewData["Categories"] = new SelectList(_context.Categories, "CategoryId", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.Categories = new SelectList(_context.Categories, "CategoryId", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,FirstName,LastName,Phone,Email,DateOfBirth,Description,Price,StockQuantity,CategoryId,IsActive")] Product product, IFormFile imageFile)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (true)
            {
                try
                {
                    var existingProduct = await _context.Products.FindAsync(id);
                    if (existingProduct == null)
                    {
                        return NotFound();
                    }

                    if (imageFile != null && imageFile.Length > 0)
                    {
                        using var memoryStream = new MemoryStream();
                        await imageFile.CopyToAsync(memoryStream);
                        existingProduct.ImageData = memoryStream.ToArray();
                    }

                    existingProduct.FirstName = product.FirstName;
                    existingProduct.LastName = product.LastName;
                    existingProduct.Phone = product.Phone;
                    existingProduct.Email = product.Email;
                    existingProduct.DateOfBirth = product.DateOfBirth;
                    existingProduct.Description = product.Description;
                    existingProduct.Price = product.Price;
                    existingProduct.StockQuantity = product.StockQuantity;
                    existingProduct.CategoryId = product.CategoryId;
                    existingProduct.IsActive = product.IsActive;
                    existingProduct.LastModifiedDate = DateTime.UtcNow;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = new SelectList(_context.Categories, "CategoryId", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Products/ExportToXml
        public async Task<IActionResult> ExportToXml()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .ToListAsync();

            var settings = new System.Xml.XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  "
            };

            using var memoryStream = new MemoryStream();
            using (var xmlWriter = System.Xml.XmlWriter.Create(memoryStream, settings))
            {
                var serializer = new XmlSerializer(typeof(List<Product>));
                serializer.Serialize(xmlWriter, products);
            }

            return File(memoryStream.ToArray(), "application/xml", "products.xml");
        }

        // GET: Products/ExportToJson
        public async Task<IActionResult> ExportToJson()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .ToListAsync();

            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented
            };

            var json = JsonConvert.SerializeObject(products, settings);
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);

            return File(bytes, "application/json", "products.json");
        }

        // GET: Products/Import
        public IActionResult Import()
        {
            return View();
        }

        // POST: Products/ImportFromXml
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportFromXml(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Please select a file to import.";
                return RedirectToAction(nameof(Import));
            }

            try
            {
                using var stream = file.OpenReadStream();
                var serializer = new XmlSerializer(typeof(List<Product>));
                var products = (List<Product>)serializer.Deserialize(stream);

                foreach (var product in products)
                {
                    product.ProductId = 0; // Reset ID for new entry
                    product.CreatedDate = DateTime.UtcNow;
                    product.LastModifiedDate = DateTime.UtcNow;
                    _context.Products.Add(product);
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = $"Successfully imported {products.Count} products.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error importing XML file: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Products/ImportFromJson
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportFromJson(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Please select a file to import.";
                return RedirectToAction(nameof(Import));
            }

            try
            {
                using var reader = new StreamReader(file.OpenReadStream());
                var json = await reader.ReadToEndAsync();
                var products = JsonConvert.DeserializeObject<List<Product>>(json);

                foreach (var product in products)
                {
                    product.ProductId = 0; // Reset ID for new entry
                    product.CreatedDate = DateTime.UtcNow;
                    product.LastModifiedDate = DateTime.UtcNow;
                    _context.Products.Add(product);
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = $"Successfully imported {products.Count} products.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error importing JSON file: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
} 