using COMP2139_assign01.Data;
using COMP2139_assign01.Models;
using COMP2139_assign01.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace COMP2139_assign01.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly CartService _cartService;

        public OrderController(ApplicationDbContext context, CartService cartService)
        {
            _context = context;
            _cartService = cartService;
        }

        // GET: Orders (Admin View)
        public async Task<IActionResult> Index(string searchTerm, DateTime? fromDate, DateTime? toDate, string sortBy)
        {
            try
            {
                var query = _context.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .AsQueryable();

                // Apply search
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query = query.Where(o =>
                        o.GuestName.Contains(searchTerm) ||
                        o.GuestEmail.Contains(searchTerm) ||
                        o.TrackingNumber.Contains(searchTerm));
                }

                // Apply date range
                if (fromDate.HasValue)
                {
                    query = query.Where(o => o.OrderDate >= fromDate.Value);
                }
                if (toDate.HasValue)
                {
                    query = query.Where(o => o.OrderDate <= toDate.Value.AddDays(1));
                }

                // Apply sorting
                query = sortBy?.ToLower() switch
                {
                    "date_desc" => query.OrderByDescending(o => o.OrderDate),
                    "date" => query.OrderBy(o => o.OrderDate),
                    "name_desc" => query.OrderByDescending(o => o.GuestName),
                    "name" => query.OrderBy(o => o.GuestName),
                    _ => query.OrderByDescending(o => o.OrderDate)
                };

                ViewBag.CurrentSearchTerm = searchTerm;
                ViewBag.CurrentFromDate = fromDate;
                ViewBag.CurrentToDate = toDate;
                ViewBag.CurrentSortBy = sortBy;

                var orders = await query.ToListAsync();
                return View(orders);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while loading orders: " + ex.Message;
                return View(new List<Order>());
            }
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Order ID is required.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .FirstOrDefaultAsync(o => o.OrderId == id);

                if (order == null)
                {
                    TempData["Error"] = "Order not found.";
                    return RedirectToAction(nameof(Index));
                }

                return View(order);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while loading the order: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Order ID is required.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .FirstOrDefaultAsync(o => o.OrderId == id);

                if (order == null)
                {
                    TempData["Error"] = "Order not found.";
                    return RedirectToAction(nameof(Index));
                }

                return View(order);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while loading the order: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,GuestName,GuestEmail,GuestPhone,TrackingNumber")] Order order)
        {
            if (id != order.OrderId)
            {
                TempData["Error"] = "Invalid order ID.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var existingOrder = await _context.Orders
                        .Include(o => o.OrderItems)
                        .FirstOrDefaultAsync(o => o.OrderId == id);

                    if (existingOrder == null)
                    {
                        TempData["Error"] = "Order not found.";
                        return RedirectToAction(nameof(Index));
                    }

                    existingOrder.GuestName = order.GuestName;
                    existingOrder.GuestEmail = order.GuestEmail;
                    existingOrder.GuestPhone = order.GuestPhone;
                    existingOrder.TrackingNumber = order.TrackingNumber;

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Order updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                return View(order);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while updating the order: " + ex.Message;
                return View(order);
            }
        }

        // POST: Orders/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.OrderId == id);

                if (order == null)
                {
                    TempData["Error"] = "Order not found.";
                    return RedirectToAction(nameof(Index));
                }

                _context.OrderItems.RemoveRange(order.OrderItems);
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Order deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while deleting the order: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // Guest Area Actions
        public IActionResult Cart()
        {
            var cartItems = _cartService.GetCart();
            return View(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            try
            {
                await _cartService.AddItem(productId, quantity);
                TempData["Success"] = "Item added to cart successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Cart");
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            try
            {
                _cartService.UpdateQuantity(productId, quantity);
                TempData["Success"] = "Cart updated successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Cart");
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            var cartItems = _cartService.GetCart();
            _cartService.RemoveItem(productId);
            TempData["CartItemCount"] = cartItems.Count;
            return RedirectToAction("Cart");
        }

        public IActionResult Checkout()
        {
            var cartItems = _cartService.GetCart();
            if (!cartItems.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Cart");
            }

            var order = new Order
            {
                OrderItems = cartItems.Select(item => new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product.Price
                }).ToList(),
                Total = _cartService.GetTotal()
            };

            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(Order order)
        {
            if (!ModelState.IsValid)
            {
                return View("Checkout", order);
            }

            var cartItems = _cartService.GetCart();
            if (!cartItems.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Cart");
            }

            order.OrderDate = DateTime.UtcNow;
            order.OrderNumber = GenerateOrderNumber();
            order.Status = OrderStatus.Pending;
            order.Total = _cartService.GetTotal();

            order.OrderItems = cartItems.Select(item => new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.Product.Price
            }).ToList();

            _context.Orders.Add(order);

            // Update product quantities
            foreach (var item in order.OrderItems)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    product.Quantity -= item.Quantity;
                }
            }

            await _context.SaveChangesAsync();
            _cartService.Clear();

            return RedirectToAction("OrderConfirmation", new { id = order.OrderId });
        }

        public async Task<IActionResult> OrderConfirmation(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        private string GenerateOrderNumber()
        {
            return DateTime.UtcNow.ToString("yyyyMMddHHmmss") + new Random().Next(1000, 9999).ToString();
        }

        public IActionResult Track()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Track(string trackingNumber)
        {
            if (string.IsNullOrEmpty(trackingNumber))
            {
                ModelState.AddModelError("", "Please enter a tracking number");
                return View();
            }

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.TrackingNumber == trackingNumber);

            if (order == null)
            {
                ModelState.AddModelError("", "Order not found");
                return View();
            }

            return View("OrderConfirmation", order);
        }
    }
}
