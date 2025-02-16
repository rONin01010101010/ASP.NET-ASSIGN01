using Microsoft.AspNetCore.Mvc;
using COMP2139_assign01.Models;
using COMP2139_assign01.Data;
using Microsoft.EntityFrameworkCore;

namespace COMP2139_assign01.Controllers
{
    public class GuestOrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GuestOrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: GuestOrder
        public IActionResult Index()
        {
            return View();
        }

        // POST: GuestOrder/Track
        [HttpPost]
        public async Task<IActionResult> Track(string trackingNumber)
        {
            if (string.IsNullOrEmpty(trackingNumber))
            {
                ModelState.AddModelError("", "Please enter a tracking number");
                return View("Index");
            }

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.TrackingNumber == trackingNumber);

            if (order == null)
            {
                ModelState.AddModelError("", "Order not found. Please check your tracking number.");
                return View("Index");
            }

            return View("Details", order);
        }

        // GET: GuestOrder/Details/[tracking-number]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction(nameof(Index));
            }

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.TrackingNumber == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
    }
}
