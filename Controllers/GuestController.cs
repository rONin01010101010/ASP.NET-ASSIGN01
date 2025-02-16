using COMP2139_assign01.Data;
using COMP2139_assign01.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace COMP2139_assign01.Controllers
{
    public class GuestController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GuestController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Guest
        public async Task<IActionResult> Index(string searchTerm, int? categoryId, 
            decimal? minPrice, decimal? maxPrice, string sortBy)
        {
            try
            {
                ViewBag.Categories = await _context.Categories.ToListAsync();
                var products = await ApplyFiltersAndSort(searchTerm, categoryId, minPrice, maxPrice, sortBy);
                
                // Store filter values for the view
                ViewBag.CurrentSearchTerm = searchTerm;
                ViewBag.CurrentCategoryId = categoryId;
                ViewBag.CurrentMinPrice = minPrice;
                ViewBag.CurrentMaxPrice = maxPrice;
                ViewBag.CurrentSortBy = sortBy;

                return View(products);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while loading products: " + ex.Message;
                return View(new List<Product>());
            }
        }

        // GET: Guest/Details/5
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

        private async Task<List<Product>> ApplyFiltersAndSort(string searchTerm, int? categoryId, 
            decimal? minPrice, decimal? maxPrice, string sortBy)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Where(p => p.Quantity > 0) // Only show in-stock items
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p => 
                    p.Name.Contains(searchTerm) || 
                    p.Description.Contains(searchTerm) || 
                    p.SKU.Contains(searchTerm));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            // Apply sorting
            query = sortBy?.ToLower() switch
            {
                "name_desc" => query.OrderByDescending(p => p.Name),
                "name" => query.OrderBy(p => p.Name),
                "price_desc" => query.OrderByDescending(p => p.Price),
                "price" => query.OrderBy(p => p.Price),
                "category_desc" => query.OrderByDescending(p => p.Category.Name),
                "category" => query.OrderBy(p => p.Category.Name),
                _ => query.OrderBy(p => p.Name)
            };

            return await query.ToListAsync();
        }
    }
}
