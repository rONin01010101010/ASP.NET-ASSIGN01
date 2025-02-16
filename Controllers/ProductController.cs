using COMP2139_assign01.Data;
using COMP2139_assign01.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace COMP2139_assign01.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index(string searchTerm, int? categoryId, 
            decimal? minPrice, decimal? maxPrice, bool? lowStock, string sortBy)
        {
            try
            {
                var query = _context.Products
                    .Include(p => p.Category)
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

                if (lowStock == true)
                {
                    query = query.Where(p => p.Quantity <= p.LowStockThreshold);
                }

                // Apply sorting
                query = sortBy?.ToLower() switch
                {
                    "name_desc" => query.OrderByDescending(p => p.Name),
                    "name" => query.OrderBy(p => p.Name),
                    "price_desc" => query.OrderByDescending(p => p.Price),
                    "price" => query.OrderBy(p => p.Price),
                    "quantity_desc" => query.OrderByDescending(p => p.Quantity),
                    "quantity" => query.OrderBy(p => p.Quantity),
                    "category_desc" => query.OrderByDescending(p => p.Category.Name),
                    "category" => query.OrderBy(p => p.Category.Name),
                    _ => query.OrderBy(p => p.Name)
                };

                var products = await query.ToListAsync();

                ViewBag.Categories = await _context.Categories.ToListAsync();
                ViewBag.CurrentSearchTerm = searchTerm;
                ViewBag.CurrentCategoryId = categoryId;
                ViewBag.CurrentMinPrice = minPrice;
                ViewBag.CurrentMaxPrice = maxPrice;
                ViewBag.CurrentLowStock = lowStock;
                ViewBag.CurrentSortBy = sortBy;

                return View(products);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while loading products: " + ex.Message;
                return View(new List<Product>());
            }
        }

        // GET: Products/Search
        [HttpGet]
        public async Task<IActionResult> Search(string searchTerm, int? categoryId, 
            decimal? minPrice, decimal? maxPrice, bool? lowStock, string sortBy)
        {
            try
            {
                var query = _context.Products
                    .Include(p => p.Category)
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

                if (lowStock == true)
                {
                    query = query.Where(p => p.Quantity <= p.LowStockThreshold);
                }

                // Apply sorting
                query = sortBy?.ToLower() switch
                {
                    "name_desc" => query.OrderByDescending(p => p.Name),
                    "name" => query.OrderBy(p => p.Name),
                    "price_desc" => query.OrderByDescending(p => p.Price),
                    "price" => query.OrderBy(p => p.Price),
                    "quantity_desc" => query.OrderByDescending(p => p.Quantity),
                    "quantity" => query.OrderBy(p => p.Quantity),
                    "category_desc" => query.OrderByDescending(p => p.Category.Name),
                    "category" => query.OrderBy(p => p.Category.Name),
                    _ => query.OrderBy(p => p.Name)
                };

                var products = await query.ToListAsync();

                ViewBag.CurrentSortBy = sortBy;
                return PartialView("_ProductList", products);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
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
        public async Task<IActionResult> Create()
        {
            try
            {
                ViewBag.Categories = await _context.Categories.ToListAsync();
                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while loading the create form: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,SKU,Price,Quantity,LowStockThreshold,CategoryId")] Product product)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Products.Add(product);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = $"Product '{product.Name}' was created successfully.";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.Categories = await _context.Categories.ToListAsync();
                TempData["Error"] = "Please correct the errors and try again.";
                return View(product);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while creating the product: " + ex.Message;
                ViewBag.Categories = await _context.Categories.ToListAsync();
                return View(product);
            }
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Product ID is required.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    TempData["Error"] = "Product not found.";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.Categories = await _context.Categories.ToListAsync();
                return View(product);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while loading the product: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,Name,Description,SKU,Price,Quantity,LowStockThreshold,CategoryId")] Product product)
        {
            if (id != product.ProductId)
            {
                TempData["Error"] = "Invalid product ID.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                if (ModelState.IsValid)
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = $"Product '{product.Name}' was updated successfully.";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.Categories = await _context.Categories.ToListAsync();
                TempData["Error"] = "Please correct the errors and try again.";
                return View(product);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(product.ProductId))
                {
                    TempData["Error"] = "Product no longer exists.";
                    return RedirectToAction(nameof(Index));
                }
                throw;
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while updating the product: " + ex.Message;
                ViewBag.Categories = await _context.Categories.ToListAsync();
                return View(product);
            }
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Product ID is required.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var product = await _context.Products
                    .Include(p => p.Category)
                    .FirstOrDefaultAsync(m => m.ProductId == id);

                if (product == null)
                {
                    TempData["Error"] = "Product not found.";
                    return RedirectToAction(nameof(Index));
                }

                return View(product);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while loading the product: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    TempData["Error"] = "Product not found.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Product '{product.Name}' was deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while deleting the product: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
