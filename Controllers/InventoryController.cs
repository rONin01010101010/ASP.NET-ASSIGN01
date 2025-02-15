using System.Net.Security;
using COMP2139_assign01.Data;
using COMP2139_assign01.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace COMP2139_assign01.Controllers
{
    public class InventoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InventoryController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public IActionResult Index()
        {
            var inventory = _context.Inventory.ToList();
            return View(inventory);
        }


        [HttpGet]
        public IActionResult Summary(Inventory inventory)
        {
            var totalStock = _context.Inventory.Sum(i => i.Quantity);
            var lowStockItems = _context.Inventory.Where(i => i.Quantity < 10).ToList();
            var categories = _context.Inventory.Select(i => i.category).Distinct().ToList();


            Inventory summary = new Inventory
            {
                LowStockItems = lowStockItems,
                category = categories.FirstOrDefault(),
                TotalStock = totalStock,
                
            };

            return View(summary);
        }

        public IActionResult Delete(int id)
        {
            var inventory = _context.Inventory.Find(id);

            if (inventory == null)
            {
                return NotFound();
            }


            _context.Inventory.Remove(inventory);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(Inventory inventory)
        {
            if (ModelState.IsValid)
            {
                _context.Inventory.Add(inventory);
                try
                {
                    _context.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    Console.WriteLine("Failed to update inventory{exception}", ex);

                }
                return RedirectToAction("Index");
            }

            return View(inventory);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var inventory = _context.Inventory.Find(id);
            if (inventory == null)
            {
                return NotFound();
            }

            return View(inventory);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("InventoryId,Quantity,Name,Price,category")] Inventory inventory)
        {
            //embed the primary key in the edit form at the top of the page (@Html.HiddenFor(model =>model.inventoryID)

            if (id != inventory.InventoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Inventory.Update(inventory);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    Console.WriteLine($"Error while updating inventory: {ex.Message}");

                    
                    if (!itemExists(inventory.InventoryId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction("Index");
            }

            return View(inventory);
        }

        private bool itemExists(int id)
        {
            return _context.Inventory.Any(e => e.InventoryId == id);
        }

        [HttpGet]
        public IActionResult Search(string Price, string Category, string Name)
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

            var result = inventoryItems.ToList();  
            return View(result);  
        }

            
        }
    }

        
        
    
