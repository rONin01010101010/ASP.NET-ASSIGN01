using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using COMP2139_assign01.Data;
using COMP2139_assign01.Models;
using COMP2139_assign01.Services;

namespace COMP2139_assign01.Areas.Guest.Controllers
{
    [Area("Guest")]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly CartService _cartService;

        public OrderController(ApplicationDbContext context, CartService cartService)
        {
            _context = context;
            _cartService = cartService;
        }

        public IActionResult Cart()
        {
            var cartItems = _cartService.GetCart();
            var viewModel = cartItems.Select(item => new CartItemViewModel
            {
                ProductId = item.ProductId,
                Name = item.Product?.Name ?? "Unknown Product",
                Price = item.Product?.Price ?? 0,
                Quantity = item.Quantity,
                Total = (item.Product?.Price ?? 0) * item.Quantity
            }).ToList();
            
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            try
            {
                if (quantity <= 0)
                {
                    return Json(new { success = false, message = "Quantity must be greater than zero" });
                }

                await _cartService.AddItem(productId, quantity);
                var cartCount = _cartService.GetItemCount();
                return Json(new { success = true, cartCount = cartCount });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
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
            if (cartItems == null || !cartItems.Any())
            {
                TempData["Error"] = "Your cart is empty";
                return RedirectToAction("Cart");
            }

            ViewBag.CartItems = cartItems;
            return View(new Order());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(Order order)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                // Validate model state
                if (!ModelState.IsValid)
                {
                    var errors = string.Join("; ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    Console.WriteLine($"Model validation failed: {errors}");
                    ViewBag.CartItems = _cartService.GetCart();
                    return View("Checkout", order);
                }

                // Get cart items
                var cartItems = _cartService.GetCart();
                Console.WriteLine($"Cart items count: {cartItems?.Count ?? 0}");
                
                if (cartItems == null || !cartItems.Any())
                {
                    TempData["Error"] = "Your cart is empty";
                    return RedirectToAction("Cart");
                }

                // Set order details
                order.OrderDate = DateTime.Now;
                order.Status = OrderStatus.Pending;
                order.OrderNumber = GenerateOrderNumber();
                Console.WriteLine($"Generated order number: {order.OrderNumber}");

                // Create order items
                order.OrderItems = cartItems.Select(item => new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product?.Price ?? 0
                }).ToList();

                // Calculate total
                order.Total = order.OrderItems.Sum(item => item.UnitPrice * item.Quantity);
                Console.WriteLine($"Order total: {order.Total}");

                // Save order
                _context.Orders.Add(order);
                var saveResult = await _context.SaveChangesAsync();
                Console.WriteLine($"SaveChanges result: {saveResult}");
                Console.WriteLine($"Order saved with ID: {order.OrderId}");

                // Verify order was saved
                var savedOrder = await _context.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.OrderId == order.OrderId);
                
                if (savedOrder == null)
                {
                    throw new Exception("Order was not saved properly");
                }

                Console.WriteLine($"Order verified in database. Items count: {savedOrder.OrderItems.Count}");

                // Commit transaction
                await transaction.CommitAsync();
                Console.WriteLine("Transaction committed");

                // Clear the cart after successful order
                _cartService.ClearCart();
                Console.WriteLine("Cart cleared");

                TempData["Success"] = "Order placed successfully!";
                return RedirectToAction("OrderConfirmation", new { id = order.OrderId });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error placing order: {ex}");
                ModelState.AddModelError("", "Error placing order: " + ex.Message);
                ViewBag.CartItems = _cartService.GetCart();
                return View("Checkout", order);
            }
        }

        public IActionResult OrderConfirmation(int id)
        {
            var order = _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefault(o => o.OrderId == id);

            if (order == null)
            {
                TempData["Error"] = "Order not found";
                return RedirectToAction("Index", "Home", new { area = "Guest" });
            }

            return View(order);
        }

        [HttpGet]
        public IActionResult GetCartCount()
        {
            try
            {
                var count = _cartService.GetItemCount();
                return Json(new { success = true, count });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        private string GenerateOrderNumber()
        {
            // Format: ORD-YYYYMMDD-XXXX where XXXX is a random number
            var date = DateTime.Now.ToString("yyyyMMdd");
            var random = new Random();
            var randomPart = random.Next(1000, 9999).ToString();
            return $"ORD-{date}-{randomPart}";
        }
    }
}
