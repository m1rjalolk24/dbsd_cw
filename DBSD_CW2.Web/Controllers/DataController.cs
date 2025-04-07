using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using DBSD_CW2.Data;
using DBSD_CW2.Data.Models;
using System.Xml;
using System.Text.Json;
using System.Data;

namespace DBSD_CW2.Web.Controllers
{
    public class DataController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public DataController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: Data
        public async Task<IActionResult> Index(string firstNameFilter, int? categoryFilter, DateTime? createdDateFilter, 
                                               string sortField = "ProductId", string sortDirection = "ASC", 
                                               int pageNumber = 1, int pageSize = 10)
        {
            ViewData["FirstNameFilter"] = firstNameFilter;
            ViewData["CategoryFilter"] = categoryFilter;
            ViewData["CreatedDateFilter"] = createdDateFilter?.ToString("yyyy-MM-dd");
            ViewData["SortField"] = sortField;
            ViewData["SortDirection"] = sortDirection;
            ViewData["PageNumber"] = pageNumber;
            ViewData["PageSize"] = pageSize;

            // Execute stored procedure for filtered data
            var firstNameParam = new SqlParameter("@FirstNameFilter", (object)firstNameFilter ?? DBNull.Value);
            var categoryParam = new SqlParameter("@CategoryFilter", (object)categoryFilter ?? DBNull.Value);
            var dateParam = new SqlParameter("@CreatedDateFilter", (object)createdDateFilter ?? DBNull.Value);
            var sortFieldParam = new SqlParameter("@SortField", sortField);
            var sortDirectionParam = new SqlParameter("@SortDirection", sortDirection);
            var pageNumberParam = new SqlParameter("@PageNumber", pageNumber);
            var pageSizeParam = new SqlParameter("@PageSize", pageSize);

            var products = await _context.Products
                .FromSqlRaw("EXEC GetFilteredProducts @FirstNameFilter, @CategoryFilter, @CreatedDateFilter, @SortField, @SortDirection, @PageNumber, @PageSize",
                    firstNameParam, categoryParam, dateParam, sortFieldParam, sortDirectionParam, pageNumberParam, pageSizeParam)
                .ToListAsync();

            // Get total count for pagination
            var countCmd = _context.Database.GetDbConnection().CreateCommand();
            countCmd.CommandText = "EXEC GetFilteredProducts @FirstNameFilter, @CategoryFilter, @CreatedDateFilter, @SortField, @SortDirection, @PageNumber, @PageSize";
            countCmd.Parameters.Add(new SqlParameter("@FirstNameFilter", (object)firstNameFilter ?? DBNull.Value));
            countCmd.Parameters.Add(new SqlParameter("@CategoryFilter", (object)categoryFilter ?? DBNull.Value));
            countCmd.Parameters.Add(new SqlParameter("@CreatedDateFilter", (object)createdDateFilter ?? DBNull.Value));
            countCmd.Parameters.Add(new SqlParameter("@SortField", sortField));
            countCmd.Parameters.Add(new SqlParameter("@SortDirection", sortDirection));
            countCmd.Parameters.Add(new SqlParameter("@PageNumber", 1)); // First page for count
            countCmd.Parameters.Add(new SqlParameter("@PageSize", int.MaxValue)); // Get all for count

            if (countCmd.Connection.State != ConnectionState.Open)
                countCmd.Connection.Open();

            var totalCount = products.Count;

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            ViewData["TotalPages"] = totalPages;

            // Prepare category dropdown for filtering
            ViewData["Categories"] = new SelectList(_context.Categories, "CategoryId", "Name", categoryFilter);

            return View(products);
        }

        // GET: Data/ExportXml
        public IActionResult ExportXml(string firstNameFilter, int? categoryFilter, DateTime? createdDateFilter)
        {
            // Execute stored procedure to get XML
            var firstNameParam = new SqlParameter("@FirstNameFilter", (object)firstNameFilter ?? DBNull.Value);
            var categoryParam = new SqlParameter("@CategoryFilter", (object)categoryFilter ?? DBNull.Value);
            var dateParam = new SqlParameter("@CreatedDateFilter", (object)createdDateFilter ?? DBNull.Value);

            var connection = _context.Database.GetDbConnection();
            var command = connection.CreateCommand();
            command.CommandText = "EXEC ExportProductsToXML @FirstNameFilter, @CategoryFilter, @CreatedDateFilter";
            command.Parameters.Add(firstNameParam);
            command.Parameters.Add(categoryParam);
            command.Parameters.Add(dateParam);

            if (connection.State != ConnectionState.Open)
                connection.Open();

            string xmlResult = "";
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    xmlResult = reader.GetString(0);
                }
            }

            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(xmlResult);
            return File(bytes, "application/xml", "products.xml");
        }

        // GET: Data/ExportJson
        public IActionResult ExportJson(string firstNameFilter, int? categoryFilter, DateTime? createdDateFilter)
        {
            // Execute stored procedure to get JSON
            var firstNameParam = new SqlParameter("@FirstNameFilter", (object)firstNameFilter ?? DBNull.Value);
            var categoryParam = new SqlParameter("@CategoryFilter", (object)categoryFilter ?? DBNull.Value);
            var dateParam = new SqlParameter("@CreatedDateFilter", (object)createdDateFilter ?? DBNull.Value);

            var connection = _context.Database.GetDbConnection();
            var command = connection.CreateCommand();
            command.CommandText = "EXEC ExportProductsToJSON @FirstNameFilter, @CategoryFilter, @CreatedDateFilter";
            command.Parameters.Add(firstNameParam);
            command.Parameters.Add(categoryParam);
            command.Parameters.Add(dateParam);

            if (connection.State != ConnectionState.Open)
                connection.Open();

            string jsonResult = "";
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    jsonResult = reader.GetString(0);
                }
            }

            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(jsonResult);
            return File(bytes, "application/json", "products.json");
        }

        // GET: Data/Import
        public IActionResult Import()
        {
            return View();
        }

        // POST: Data/ImportXml
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportXml(IFormFile xmlFile)
        {
            if (xmlFile == null || xmlFile.Length == 0)
            {
                ModelState.AddModelError("", "Please select an XML file to import.");
                return View("Import");
            }

            try
            {
                string xmlData;
                using (var reader = new StreamReader(xmlFile.OpenReadStream()))
                {
                    xmlData = await reader.ReadToEndAsync();
                }

                // Create XML parameter
                var xmlParam = new SqlParameter("@XmlData", SqlDbType.Xml)
                {
                    Value = xmlData
                };

                // Execute stored procedure
                var result = await _context.Database
                    .ExecuteSqlRawAsync("EXEC ImportProductsFromXML @XmlData", xmlParam);

                TempData["SuccessMessage"] = "XML data imported successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error importing XML data: {ex.Message}");
                return View("Import");
            }
        }

        // POST: Data/ImportJson
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportJson(IFormFile jsonFile)
        {
            if (jsonFile == null || jsonFile.Length == 0)
            {
                ModelState.AddModelError("", "Please select a JSON file to import.");
                return View("Import");
            }

            try
            {
                string jsonData;
                using (var reader = new StreamReader(jsonFile.OpenReadStream()))
                {
                    jsonData = await reader.ReadToEndAsync();
                }

                // Create JSON parameter
                var jsonParam = new SqlParameter("@JsonData", SqlDbType.NVarChar, -1)
                {
                    Value = jsonData
                };

                // Execute stored procedure
                var result = await _context.Database
                    .ExecuteSqlRawAsync("EXEC ImportProductsFromJSON @JsonData", jsonParam);

                TempData["SuccessMessage"] = "JSON data imported successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error importing JSON data: {ex.Message}");
                return View("Import");
            }
        }
    }
} 