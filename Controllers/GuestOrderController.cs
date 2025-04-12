using Microsoft.AspNetCore.Mvc;
using COMP2139_assign01.Models;
using COMP2139_assign01.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace COMP2139_assign01.Controllers
{
    public class GuestOrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GuestOrderController> _logger;

        public GuestOrderController(ApplicationDbContext context, ILogger<GuestOrderController> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET: GuestOrder
        public IActionResult Index()
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            _logger.LogInformation("User accessing order tracking page. RequestId: {RequestId}", requestId);
            
            return View();
        }

        // POST: GuestOrder/Track
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Track(string trackingNumber)
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(trackingNumber))
                {
                    _logger.LogWarning("Order tracking attempted with empty tracking number. RequestId: {RequestId}", requestId);
                    ModelState.AddModelError("", "Please enter a valid tracking number");
                    return View("Index");
                }
                
                // Sanitize input
                trackingNumber = trackingNumber.Trim();
                
                _logger.LogInformation("User searching for order with tracking number: {TrackingNumber}. RequestId: {RequestId}", 
                    trackingNumber, requestId);

                Order order;
                try
                {
                    // Execute query with timeout protection
                    var task = _context.Orders
                        .Include(o => o.OrderItems)
                        .ThenInclude(i => i.Product)
                        .FirstOrDefaultAsync(o => o.TrackingNumber == trackingNumber);
                    
                    // Add a timeout to prevent long-running queries
                    if (await Task.WhenAny(task, Task.Delay(5000)) != task)
                    {
                        _logger.LogWarning("Database query timeout while searching for tracking number: {TrackingNumber}. RequestId: {RequestId}", 
                            trackingNumber, requestId);
                        
                        ModelState.AddModelError("", "The search operation timed out. Please try again later.");
                        return View("Index");
                    }
                    
                    order = await task;
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Database error occurred while tracking order: {TrackingNumber}. RequestId: {RequestId}", 
                        trackingNumber, requestId);
                    
                    ModelState.AddModelError("", "A database error occurred. Please try again later.");
                    return View("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error occurred while tracking order: {TrackingNumber}. RequestId: {RequestId}", 
                        trackingNumber, requestId);
                    
                    ModelState.AddModelError("", "An error occurred while searching for your order. Please try again later.");
                    return View("Index");
                }

                if (order == null)
                {
                    _logger.LogWarning("No order found with tracking number: {TrackingNumber}. RequestId: {RequestId}", 
                        trackingNumber, requestId);
                    
                    ModelState.AddModelError("", "Order not found. Please check your tracking number and try again.");
                    return View("Index");
                }

                _logger.LogInformation("Successfully found order {OrderId} with tracking number: {TrackingNumber}. RequestId: {RequestId}", 
                    order.OrderId, trackingNumber, requestId);
                
                return View("Details", order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in Track action for tracking number: {TrackingNumber}. RequestId: {RequestId}", 
                    trackingNumber ?? "null", requestId);
                
                ModelState.AddModelError("", "An unexpected error occurred. Please try again later.");
                return View("Index");
            }
        }

        // GET: GuestOrder/Details/[tracking-number]
        public async Task<IActionResult> Details(string id)
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    _logger.LogWarning("Order details requested with empty tracking number. RequestId: {RequestId}", requestId);
                    TempData["Error"] = "Tracking number is required to view order details.";
                    return RedirectToAction(nameof(Index));
                }
                
                // Sanitize input
                id = id.Trim();
                
                _logger.LogInformation("User requesting details for order with tracking number: {TrackingNumber}. RequestId: {RequestId}", 
                    id, requestId);

                Order order;
                try
                {
                    order = await _context.Orders
                        .Include(o => o.OrderItems)
                        .ThenInclude(i => i.Product)
                        .FirstOrDefaultAsync(o => o.TrackingNumber == id);
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Database error occurred while retrieving order details for tracking number: {TrackingNumber}. RequestId: {RequestId}", 
                        id, requestId);
                    
                    TempData["Error"] = "A database error occurred while retrieving order details. Please try again later.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while retrieving order details for tracking number: {TrackingNumber}. RequestId: {RequestId}", 
                        id, requestId);
                    
                    TempData["Error"] = "An error occurred while retrieving order details. Please try again later.";
                    return RedirectToAction(nameof(Index));
                }

                if (order == null)
                {
                    _logger.LogWarning("No order found with tracking number: {TrackingNumber}. RequestId: {RequestId}", 
                        id, requestId);
                    
                    TempData["Error"] = "Order not found. Please check your tracking number.";
                    return RedirectToAction(nameof(Index));
                }

                _logger.LogInformation("Successfully retrieved details for order {OrderId} with tracking number: {TrackingNumber}. RequestId: {RequestId}", 
                    order.OrderId, id, requestId);
                
                return View(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in Details action for tracking number: {TrackingNumber}. RequestId: {RequestId}", 
                    id ?? "null", requestId);
                
                TempData["Error"] = "An unexpected error occurred while retrieving order details. Please try again later.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}