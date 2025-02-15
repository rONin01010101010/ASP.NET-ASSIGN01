using COMP2139_assign01.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using COMP2139_assign01.Models;

namespace COMP2139_assign01.Controllers;

public class InventoryTaskController : Controller
{

    private readonly ApplicationDbContext _context;

    public InventoryTaskController(ApplicationDbContext context)
    {
        _context = context;
    }




    public IActionResult Index(int projectId)
    {
        var tasks = _context.Tasks
            .Where(t => t.InventoryId == projectId)
            .ToList();

        ViewBag.ProjectId = projectId;
        return View(tasks);
    }


    public IActionResult Details(int id)
    {
        var task = _context
            .Tasks
            .Include(t => t.InventoryId) //Include the related project for the task 
            .FirstOrDefault(t => t.InventoryId == id);

        if (task == null)
        {
            return NotFound();
        }

        return View(task);
    }

    [HttpGet]
    public IActionResult Create(int inventoryId)
    {

        var inventory = _context.Inventory.Find(inventoryId);
        if (inventory == null)
        {
            return NotFound();
        }

        var task = new InventoryTask
        {
            InventoryId = inventoryId,
            Price = 0,
            category = "",
            Quantity = 0,
        };
        return View(task);

    }

    [HttpPost]
    public IActionResult Create([Bind("Name", "Category", "Price","inventoryId")] InventoryTask task)
    {
        if (ModelState.IsValid)
        {
            _context.Tasks.Add(task);
            _context.SaveChanges();
            return RedirectToAction("Index", new { InventoryId = task.InventoryId });
        }
        
        return View(task);
    }
    
    [HttpGet]
    public IActionResult Edit(int id)
    {
        var task = _context
            .Tasks
            .Include(t => t.InventoryId)
            .FirstOrDefault(t => t.InventoryId == id);

        if (task == null)
        {
            return NotFound();
        }
        return View(task);
        
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, [Bind("InventoryId", "Name", "Category", "InventoryId" , "Price" , "Quantity")] InventoryTask task)
    {
        if (id != task.InventoryId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            _context.Tasks.Update(task);
            _context.SaveChanges();
            return RedirectToAction("Index", new { projectId = task.InventoryId });
        }
        return View(task);
    }
    
    public IActionResult Delete(int id)
    {
        var task = _context
            .Tasks
            .Include(p => p.Inventory)
            .FirstOrDefault(t => t.InventoryId == id);

        if (task == null)
        {
            return NotFound();
        }
        return View(task);
    }
    
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        
        var task = _context.Tasks.Find(id);
        if (task != null)
        {
            _context.Tasks.Remove(task);
            _context.SaveChanges();
            return RedirectToAction("Index", new { id = task.InventoryId });
        }
        
        return NotFound();
    }
    
}