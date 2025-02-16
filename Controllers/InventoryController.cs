using System.Net.Security;
using COMP2139_assign01.Data;
using COMP2139_assign01.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace COMP2139_assign01.Controllers
{
    public class InventoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<InventoryController> _logger;

        public InventoryController(ApplicationDbContext context, ILogger<InventoryController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var inventory = await _context.Inventory
                    .OrderBy(i => i.category)
                    .ThenBy(i => i.Name)
                    .ToListAsync();
                return View(inventory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving inventory list");
                TempData["Error"] = "Failed to retrieve inventory list.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Summary()
        {
            try
            {
                var totalStock = await _context.Inventory.SumAsync(i => i.Quantity);
                var lowStockItems = await _context.Inventory
                    .Where(i => i.Quantity < 10)
                    .OrderBy(i => i.Quantity)
                    .ToListAsync();
                var categories = await _context.Inventory
                    .Select(i => i.category)
                    .Distinct()
                    .ToListAsync();

                var summary = new Inventory
                {
                    TotalStock = totalStock,
                    category = categories.FirstOrDefault() ?? string.Empty,
                    Quantity = lowStockItems.Count
                };

                ViewBag.LowStockItems = lowStockItems;
                return View(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving inventory summary");
                TempData["Error"] = "Failed to retrieve inventory summary.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var inventory = await _context.Inventory.FindAsync(id);
                if (inventory == null)
                {
                    TempData["Error"] = "Item not found.";
                    return RedirectToAction(nameof(Index));
                }

                return View(inventory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving item for deletion");
                TempData["Error"] = "Failed to retrieve item for deletion.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var inventory = await _context.Inventory.FindAsync(id);
                if (inventory == null)
                {
                    TempData["Error"] = "Item not found.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Inventory.Remove(inventory);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Item '{inventory.Name}' has been deleted.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting item");
                TempData["Error"] = "Failed to delete item. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Inventory inventory)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Inventory.Add(inventory);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = $"Item '{inventory.Name}' has been added successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Error adding item");
                    ModelState.AddModelError("", "Failed to add item. Please try again.");
                }
            }

            return View(inventory);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var inventory = await _context.Inventory.FindAsync(id);
                if (inventory == null)
                {
                    TempData["Error"] = "Item not found.";
                    return RedirectToAction(nameof(Index));
                }

                return View(inventory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving item for editing");
                TempData["Error"] = "Failed to retrieve item for editing.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("InventoryId,Quantity,Name,Price,category")] Inventory inventory)
        {
            if (id != inventory.InventoryId)
            {
                TempData["Error"] = "Invalid request.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inventory);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = $"Item '{inventory.Name}' has been updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError(ex, "Error updating item");
                    if (!ItemExists(inventory.InventoryId))
                    {
                        TempData["Error"] = "Item not found.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("", "Failed to update item. Please try again.");
                    }
                }
            }

            return View(inventory);
        }

        private bool ItemExists(int id)
        {
            return _context.Inventory.Any(e => e.InventoryId == id);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string Price, string Category, string Name)
        {
            try
            {
                ViewBag.Price = Price;
                ViewBag.Category = Category;
                ViewBag.Name = Name;

                var inventoryItems = _context.Inventory.AsQueryable();

                if (decimal.TryParse(Price, out decimal price))
                {
                    inventoryItems = inventoryItems.Where(i => i.Price <= price);
                }

                if (!string.IsNullOrEmpty(Category))
                {
                    inventoryItems = inventoryItems.Where(i => i.category.Contains(Category));
                }

                if (!string.IsNullOrEmpty(Name))
                {
                    inventoryItems = inventoryItems.Where(i => i.Name.Contains(Name));
                }

                var result = await inventoryItems
                    .OrderBy(i => i.category)
                    .ThenBy(i => i.Name)
                    .ToListAsync();

                return View(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching inventory");
                TempData["Error"] = "Failed to search inventory.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
