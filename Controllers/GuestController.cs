using COMP2139_assign01.Data;
using COMP2139_assign01.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace COMP2139_assign01.Controllers
{
    public class GuestController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GuestController> _logger;

        public GuestController(ApplicationDbContext context, ILogger<GuestController> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET: Guest
        public async Task<IActionResult> Index(string searchTerm, int? categoryId, 
            decimal? minPrice, decimal? maxPrice, string sortBy)
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            
            try
            {
                _logger.LogInformation("User accessing product catalog with filters: SearchTerm='{SearchTerm}', CategoryId={CategoryId}, " +
                                      "MinPrice={MinPrice}, MaxPrice={MaxPrice}, SortBy='{SortBy}'. RequestId: {RequestId}", 
                    searchTerm ?? "none", categoryId, minPrice, maxPrice, sortBy ?? "default", requestId);
                
                // Get categories first, with error handling
                List<Category> categories;
                try
                {
                    categories = await _context.Categories.ToListAsync();
                    ViewBag.Categories = categories;
                    
                    _logger.LogDebug("Successfully loaded {Count} categories. RequestId: {RequestId}", categories.Count, requestId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error loading categories. RequestId: {RequestId}", requestId);
                    ViewBag.Categories = new List<Category>(); // Provide empty list to avoid null reference
                    TempData["Error"] = "Could not load product categories. Some filtering options may be unavailable.";
                }
                
                // Validate price range
                if (minPrice.HasValue && maxPrice.HasValue && minPrice > maxPrice)
                {
                    _logger.LogWarning("Invalid price range specified: Min={MinPrice}, Max={MaxPrice}. RequestId: {RequestId}", 
                        minPrice, maxPrice, requestId);
                    
                    // Swap values to make a valid range
                    var temp = minPrice;
                    minPrice = maxPrice;
                    maxPrice = temp;
                    
                    TempData["Warning"] = "The minimum price was higher than the maximum price. Values have been swapped.";
                }
                
                // Apply filters and sorting with error handling
                List<Product> products;
                try
                {
                    products = await ApplyFiltersAndSort(searchTerm, categoryId, minPrice, maxPrice, sortBy);
                    
                    _logger.LogInformation("Successfully retrieved {Count} products matching the specified filters. RequestId: {RequestId}", 
                        products.Count, requestId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error retrieving filtered products. RequestId: {RequestId}", requestId);
                    products = new List<Product>(); // Return empty list
                    TempData["Error"] = "An error occurred while loading products. Please try again later.";
                }
                
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
                _logger.LogError(ex, "Unexpected error in Guest/Index. RequestId: {RequestId}", requestId);
                TempData["Error"] = "An unexpected error occurred. Please try again later.";
                return View(new List<Product>());
            }
        }

        // GET: Guest/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            
            try
            {
                if (id == null)
                {
                    _logger.LogWarning("Product details requested without an ID. RequestId: {RequestId}", requestId);
                    return BadRequest("Product ID is required.");
                }

                _logger.LogInformation("User requesting details for product ID {ProductId}. RequestId: {RequestId}", 
                    id, requestId);
                
                Product product;
                try
                {
                    product = await _context.Products
                        .Include(p => p.Category)
                        .FirstOrDefaultAsync(m => m.ProductId == id);
                    
                    if (product == null)
                    {
                        _logger.LogWarning("Product with ID {ProductId} not found. RequestId: {RequestId}", 
                            id, requestId);
                        return NotFound($"Product with ID {id} not found.");
                    }
                    
                    _logger.LogDebug("Successfully retrieved product {ProductName} (ID: {ProductId}). RequestId: {RequestId}", 
                        product.Name, product.ProductId, requestId);
                    
                    return View(product);
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Database error retrieving product ID {ProductId}. RequestId: {RequestId}", 
                        id, requestId);
                    TempData["Error"] = "A database error occurred while retrieving the product. Please try again later.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error retrieving product ID {ProductId}. RequestId: {RequestId}", 
                        id, requestId);
                    TempData["Error"] = "An error occurred while retrieving the product. Please try again later.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in Guest/Details for product ID {ProductId}. RequestId: {RequestId}", 
                    id, requestId);
                TempData["Error"] = "An unexpected error occurred. Please try again later.";
                return RedirectToAction(nameof(Index));
            }
        }

        private async Task<List<Product>> ApplyFiltersAndSort(string searchTerm, int? categoryId, 
            decimal? minPrice, decimal? maxPrice, string sortBy)
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            
            try
            {
                _logger.LogDebug("Applying filters: SearchTerm='{SearchTerm}', CategoryId={CategoryId}, " +
                                "MinPrice={MinPrice}, MaxPrice={MaxPrice}, SortBy='{SortBy}'. RequestId: {RequestId}", 
                    searchTerm ?? "none", categoryId, minPrice, maxPrice, sortBy ?? "default", requestId);
                
                var query = _context.Products
                    .Include(p => p.Category)
                    .AsQueryable();
                
                try
                {
                    // First apply in-stock filter
                    query = query.Where(p => p.Quantity > 0);
                    
                    // Apply search term filter with error handling
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        searchTerm = searchTerm.Trim();
                        query = query.Where(p => 
                            p.Name.Contains(searchTerm) || 
                            p.Description.Contains(searchTerm) || 
                            p.SKU.Contains(searchTerm));
                    }

                    // Apply category filter
                    if (categoryId.HasValue)
                    {
                        // Verify category exists first
                        bool categoryExists = await _context.Categories.AnyAsync(c => c.CategoryId == categoryId);
                        if (categoryExists)
                        {
                            query = query.Where(p => p.CategoryId == categoryId);
                        }
                        else
                        {
                            _logger.LogWarning("Filter specified non-existent category ID {CategoryId}. RequestId: {RequestId}", 
                                categoryId, requestId);
                            // Continue without applying this filter
                        }
                    }

                    // Apply price range filters
                    if (minPrice.HasValue)
                    {
                        if (minPrice.Value < 0)
                        {
                            _logger.LogWarning("Negative minimum price {MinPrice} specified. Using 0 instead. RequestId: {RequestId}", 
                                minPrice, requestId);
                            minPrice = 0;
                        }
                        query = query.Where(p => p.Price >= minPrice.Value);
                    }

                    if (maxPrice.HasValue)
                    {
                        if (maxPrice.Value < 0)
                        {
                            _logger.LogWarning("Negative maximum price {MaxPrice} specified. Ignoring max price filter. RequestId: {RequestId}", 
                                maxPrice, requestId);
                        }
                        else
                        {
                            query = query.Where(p => p.Price <= maxPrice.Value);
                        }
                    }

                    // Apply sorting with fallback for invalid sort parameters
                    query = ApplySorting(query, sortBy);
                    
                    _logger.LogDebug("Executing database query for filtered products. RequestId: {RequestId}", requestId);
                    var result = await query.ToListAsync();
                    
                    _logger.LogDebug("Query returned {Count} products. RequestId: {RequestId}", result.Count, requestId);
                    return result;
                }
                catch (InvalidOperationException ex)
                {
                    _logger.LogError(ex, "Invalid operation error while filtering products. RequestId: {RequestId}", requestId);
                    throw new ApplicationException("An error occurred while filtering products.", ex);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error applying filters and sorting. RequestId: {RequestId}", requestId);
                    throw new ApplicationException("An error occurred while applying product filters.", ex);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in ApplyFiltersAndSort. RequestId: {RequestId}", requestId);
                throw; // Re-throw to be handled by the calling method
            }
        }
        
        private IQueryable<Product> ApplySorting(IQueryable<Product> query, string sortBy)
        {
            // Default sort is by name if sortBy is null or invalid
            if (string.IsNullOrEmpty(sortBy))
            {
                return query.OrderBy(p => p.Name);
            }
            
            try
            {
                // Use switch expression with safe fallback
                return sortBy.ToLower() switch
                {
                    "name_desc" => query.OrderByDescending(p => p.Name),
                    "name" => query.OrderBy(p => p.Name),
                    "price_desc" => query.OrderByDescending(p => p.Price),
                    "price" => query.OrderBy(p => p.Price),
                    "category_desc" => query.OrderByDescending(p => p.Category.Name),
                    "category" => query.OrderBy(p => p.Category.Name),
                    _ => query.OrderBy(p => p.Name) // Default sort for unrecognized options
                };
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error applying sort '{SortBy}'. Falling back to default sort.", sortBy);
                return query.OrderBy(p => p.Name); // Fallback to default sort on error
            }
        }
    }
}