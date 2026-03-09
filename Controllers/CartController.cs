using Microsoft.AspNetCore.Mvc;
using NeoGlassCommerce.Services;

namespace NeoGlassCommerce.Controllers
{
    public class CartController : Controller
    {
        private readonly CartService _cartService;

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        public IActionResult Index()
        {
            var cart = _cartService.GetCart();
            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> Add(int productId, int quantity = 1)
        {
            await _cartService.AddToCartAsync(productId, quantity);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Update(int productId, int qty)
        {
            _cartService.UpdateQuantity(productId, qty);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Remove(int productId)
        {
            _cartService.RemoveFromCart(productId);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult GetCartCount()
        {
            return Json(new { count = _cartService.GetCartItemCount() });
        }
    }
}
