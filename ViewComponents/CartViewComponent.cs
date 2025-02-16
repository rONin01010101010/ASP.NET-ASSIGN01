using Microsoft.AspNetCore.Mvc;
using COMP2139_assign01.Services;

namespace COMP2139_assign01.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        private readonly CartService _cartService;

        public CartViewComponent(CartService cartService)
        {
            _cartService = cartService;
        }

        public IViewComponentResult Invoke()
        {
            var count = _cartService.GetItemCount();
            return View("Default", count);
        }
    }
}
