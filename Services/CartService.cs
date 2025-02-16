using COMP2139_assign01.Data;
using COMP2139_assign01.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace COMP2139_assign01.Services
{
    public class CartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _context;
        private const string CartSessionKey = "Cart";

        public CartService(IHttpContextAccessor httpContextAccessor, ApplicationDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        private ISession Session => _httpContextAccessor.HttpContext.Session;

        public async Task AddItem(int productId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                throw new ArgumentException("Product not found");

            if (product.Quantity < quantity)
                throw new ArgumentException("Not enough stock available");

            var cart = GetCart();
            var cartItem = cart.FirstOrDefault(item => item.ProductId == productId);

            if (cartItem != null)
            {
                if (cartItem.Quantity + quantity > product.Quantity)
                    throw new ArgumentException("Not enough stock available");
                
                cartItem.Quantity += quantity;
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductId = productId,
                    Quantity = quantity
                });
            }

            SaveCart(cart);
        }

        public void RemoveItem(int productId)
        {
            var cart = GetCart();
            var cartItem = cart.FirstOrDefault(item => item.ProductId == productId);
            if (cartItem != null)
            {
                cart.Remove(cartItem);
                SaveCart(cart);
            }
        }

        public void UpdateQuantity(int productId, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero");

            var product = _context.Products.Find(productId);
            if (product == null)
                throw new ArgumentException("Product not found");

            if (product.Quantity < quantity)
                throw new ArgumentException("Not enough stock available");

            var cart = GetCart();
            var cartItem = cart.FirstOrDefault(item => item.ProductId == productId);
            if (cartItem != null)
            {
                cartItem.Quantity = quantity;
                SaveCart(cart);
            }
        }

        public void Clear()
        {
            Session.Remove(CartSessionKey);
        }

        public void ClearCart()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session != null)
            {
                session.Remove(CartSessionKey);
            }
        }

        public List<CartItem> GetCart()
        {
            var cartJson = Session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(cartJson))
                return new List<CartItem>();

            var cart = JsonSerializer.Deserialize<List<CartItem>>(cartJson);
            
            // Load products for each cart item
            foreach (var item in cart)
            {
                var product = _context.Products.Find(item.ProductId);
                if (product != null)
                {
                    item.Product = product;
                }
            }
            
            return cart;
        }

        public int GetItemCount()
        {
            var cart = GetCart();
            return cart?.Sum(item => item.Quantity) ?? 0;
        }

        public decimal GetTotal()
        {
            var cart = GetCart();
            return cart.Sum(item => {
                var product = _context.Products.Find(item.ProductId);
                return product?.Price * item.Quantity ?? 0;
            });
        }

        private void SaveCart(List<CartItem> cart)
        {
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            };
            
            var cartJson = JsonSerializer.Serialize(cart, options);
            Session.SetString(CartSessionKey, cartJson);
        }
    }
}
