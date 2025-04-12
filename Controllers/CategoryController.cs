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
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ApplicationDbContext context, ILogger<CategoryController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            
            try
            {
                _logger.LogInformation("User {UserId} requested category list. RequestId: {RequestId}", 
                    User.Identity?.Name ?? "Anonymous", requestId);
                
                var categories = await _context.Categories
                    .Include(c => c.Products)
                    .ToListAsync();
                
                _logger.LogInformation("Successfully retrieved {Count} categories. RequestId: {RequestId}", 
                    categories.Count, requestId);
                
                return View(categories);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error occurred while retrieving categories. RequestId: {RequestId}", requestId);
                TempData["Error"] = "A database error occurred while loading categories. Please try again later.";
                return View(new List<Category>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving categories. RequestId: {RequestId}", requestId);
                TempData["Error"] = "An unexpected error occurred while loading categories. Please try again later.";
                return View(new List<Category>());
            }
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            _logger.LogInformation("User {UserId} accessed category creation form. RequestId: {RequestId}", 
                User.Identity?.Name ?? "Anonymous", requestId);
            
            return View();
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description")] Category category)
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            
            try
            {
                _logger.LogInformation("User {UserId} attempting to create category '{CategoryName}'. RequestId: {RequestId}", 
                    User.Identity?.Name ?? "Anonymous", category.Name, requestId);
                
                if (ModelState.IsValid)
                {
                    // Check for duplicate category name
                    if (await _context.Categories.AnyAsync(c => c.Name == category.Name))
                    {
                        _logger.LogWarning("Attempt to create duplicate category '{CategoryName}'. RequestId: {RequestId}", 
                            category.Name, requestId);
                        
                        ModelState.AddModelError("Name", "A category with this name already exists.");
                        return View(category);
                    }
                    
                    // Use a transaction for data consistency
                    using var transaction = await _context.Database.BeginTransactionAsync();
                    
                    try
                    {
                        _context.Categories.Add(category);
                        await _context.SaveChangesAsync();
                        
                        await transaction.CommitAsync();
                        
                        _logger.LogInformation("Category '{CategoryName}' created successfully with ID {CategoryId}. RequestId: {RequestId}", 
                            category.Name, category.CategoryId, requestId);
                        
                        TempData["Success"] = $"Category '{category.Name}' was created successfully.";
                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception ex)
                    {
                        // Roll back the transaction on error
                        await transaction.RollbackAsync();
                        throw; // Re-throw to be caught by outer catch
                    }
                }
                
                _logger.LogWarning("Invalid model state when creating category. RequestId: {RequestId}", requestId);
                return View(category);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error occurred while creating category '{CategoryName}'. RequestId: {RequestId}", 
                    category.Name, requestId);
                
                ModelState.AddModelError("", "A database error occurred. The category could not be created.");
                return View(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while creating category '{CategoryName}'. RequestId: {RequestId}", 
                    category.Name, requestId);
                
                ModelState.AddModelError("", "An unexpected error occurred. Please try again later.");
                return View(category);
            }
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            
            if (id == null)
            {
                _logger.LogWarning("Edit category attempted with null ID. RequestId: {RequestId}", requestId);
                TempData["Error"] = "Category ID is required.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _logger.LogInformation("User {UserId} attempting to edit category with ID {CategoryId}. RequestId: {RequestId}", 
                    User.Identity?.Name ?? "Anonymous", id, requestId);
                
                var category = await _context.Categories.FindAsync(id);
                
                if (category == null)
                {
                    _logger.LogWarning("Category with ID {CategoryId} not found during edit. RequestId: {RequestId}", 
                        id, requestId);
                    
                    TempData["Error"] = "Category not found.";
                    return RedirectToAction(nameof(Index));
                }

                return View(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving category ID {CategoryId} for editing. RequestId: {RequestId}", 
                    id, requestId);
                
                TempData["Error"] = "An error occurred while loading the category. Please try again later.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoryId,Name,Description")] Category category)
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            
            if (id != category.CategoryId)
            {
                _logger.LogWarning("ID mismatch during category edit. URL ID: {UrlId}, Model ID: {ModelId}. RequestId: {RequestId}", 
                    id, category.CategoryId, requestId);
                
                TempData["Error"] = "Invalid category ID.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _logger.LogInformation("User {UserId} attempting to update category ID {CategoryId}. RequestId: {RequestId}", 
                    User.Identity?.Name ?? "Anonymous", id, requestId);
                
                if (ModelState.IsValid)
                {
                    // Check for duplicate name but exclude the current category
                    if (await _context.Categories.AnyAsync(c => c.Name == category.Name && c.CategoryId != category.CategoryId))
                    {
                        _logger.LogWarning("Attempt to update category to an existing name '{CategoryName}'. RequestId: {RequestId}", 
                            category.Name, requestId);
                        
                        ModelState.AddModelError("Name", "A category with this name already exists.");
                        return View(category);
                    }
                    
                    // Use a transaction for data consistency
                    using var transaction = await _context.Database.BeginTransactionAsync();
                    
                    try
                    {
                        _context.Update(category);
                        await _context.SaveChangesAsync();
                        
                        await transaction.CommitAsync();
                        
                        _logger.LogInformation("Category ID {CategoryId} updated successfully. RequestId: {RequestId}", 
                            category.CategoryId, requestId);
                        
                        TempData["Success"] = $"Category '{category.Name}' was updated successfully.";
                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception ex)
                    {
                        // Roll back the transaction on error
                        await transaction.RollbackAsync();
                        throw; // Re-throw to be caught by outer catch
                    }
                }
                
                _logger.LogWarning("Invalid model state when updating category ID {CategoryId}. RequestId: {RequestId}", 
                    id, requestId);
                
                return View(category);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!await CategoryExistsAsync(category.CategoryId))
                {
                    _logger.LogWarning("Concurrency exception: Category ID {CategoryId} no longer exists. RequestId: {RequestId}", 
                        category.CategoryId, requestId);
                    
                    TempData["Error"] = "Category no longer exists.";
                    return RedirectToAction(nameof(Index));
                }
                
                _logger.LogError(ex, "Concurrency error occurred while updating category ID {CategoryId}. RequestId: {RequestId}", 
                    category.CategoryId, requestId);
                
                ModelState.AddModelError("", "This category has been modified by another user. Please reload and try again.");
                return View(category);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error occurred while updating category ID {CategoryId}. RequestId: {RequestId}", 
                    category.CategoryId, requestId);
                
                ModelState.AddModelError("", "A database error occurred. The category could not be updated.");
                return View(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while updating category ID {CategoryId}. RequestId: {RequestId}", 
                    category.CategoryId, requestId);
                
                ModelState.AddModelError("", "An unexpected error occurred. Please try again later.");
                return View(category);
            }
        }

        // POST: Categories/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            
            try
            {
                _logger.LogInformation("User {UserId} attempting to delete category ID {CategoryId}. RequestId: {RequestId}", 
                    User.Identity?.Name ?? "Anonymous", id, requestId);
                
                var category = await _context.Categories
                    .Include(c => c.Products)
                    .FirstOrDefaultAsync(c => c.CategoryId == id);

                if (category == null)
                {
                    _logger.LogWarning("Category ID {CategoryId} not found during delete attempt. RequestId: {RequestId}", 
                        id, requestId);
                    
                    TempData["Error"] = "Category not found.";
                    return RedirectToAction(nameof(Index));
                }

                if (category.Products.Any())
                {
                    _logger.LogWarning("Attempted to delete category '{CategoryName}' (ID {CategoryId}) with {ProductCount} associated products. RequestId: {RequestId}", 
                        category.Name, category.CategoryId, category.Products.Count, requestId);
                    
                    TempData["Error"] = $"Cannot delete category '{category.Name}' because it contains products. Please move or delete the products first.";
                    return RedirectToAction(nameof(Index));
                }

                // Use a transaction for data consistency
                using var transaction = await _context.Database.BeginTransactionAsync();
                
                try
                {
                    _context.Categories.Remove(category);
                    await _context.SaveChangesAsync();
                    
                    await transaction.CommitAsync();
                    
                    _logger.LogInformation("Category '{CategoryName}' (ID {CategoryId}) deleted successfully. RequestId: {RequestId}", 
                        category.Name, category.CategoryId, requestId);
                    
                    TempData["Success"] = $"Category '{category.Name}' was deleted successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Roll back the transaction on error
                    await transaction.RollbackAsync();
                    throw; // Re-throw to be caught by outer catch
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error occurred while deleting category ID {CategoryId}. RequestId: {RequestId}", 
                    id, requestId);
                
                TempData["Error"] = "A database error occurred while deleting the category. Please try again later.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while deleting category ID {CategoryId}. RequestId: {RequestId}", 
                    id, requestId);
                
                TempData["Error"] = "An unexpected error occurred while deleting the category. Please try again later.";
                return RedirectToAction(nameof(Index));
            }
        }

        // Change to async version for better error handling
        private async Task<bool> CategoryExistsAsync(int id)
        {
            try
            {
                return await _context.Categories.AnyAsync(e => e.CategoryId == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if category ID {CategoryId} exists", id);
                throw; // Rethrow to be handled by calling method
            }
        }
    }
}