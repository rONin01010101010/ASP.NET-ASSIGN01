using COMP2139_assign01.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace COMP2139_assign01.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private static readonly Random _random = new Random();

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/products/random
        [HttpGet("random")]
        public async Task<IActionResult> GetRandomProduct()
        {
            var products = await _context.Products.ToListAsync();
            if (!products.Any())
            {
                return NotFound();
            }

            var randomProduct = products[_random.Next(products.Count)];
            return Ok(new { 
                productId = randomProduct.ProductId,
                name = randomProduct.Name
            });
        }
    }
}
