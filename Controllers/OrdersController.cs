using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NeoGlassCommerce.Models;
using NeoGlassCommerce.Services;
using NeoGlassCommerce.ViewModels;

namespace NeoGlassCommerce.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly OrderService _orderService;
        private readonly CartService _cartService;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrdersController(
            OrderService orderService,
            CartService cartService,
            UserManager<ApplicationUser> userManager)
        {
            _orderService = orderService;
            _cartService = cartService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User)!;
            var orders = await _orderService.GetUserOrdersAsync(userId);
            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var vm = await _orderService.GetOrderDetailsAsync(id, userId);
            if (vm == null) return NotFound();
            return View(vm);
        }

        [HttpGet]
        public IActionResult Checkout()
        {
            var cart = _cartService.GetCart();
            if (!cart.Items.Any())
                return RedirectToAction("Index", "Cart");

            var vm = new CheckoutVM { Cart = cart };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutVM vm)
        {
            vm.Cart = _cartService.GetCart();

            if (!vm.Cart.Items.Any())
            {
                ModelState.AddModelError("", "Your cart is empty.");
                return View(vm);
            }

            if (!ModelState.IsValid)
                return View(vm);

            try
            {
                var userId = _userManager.GetUserId(User)!;
                var order = await _orderService.PlaceOrderAsync(userId, vm);
                TempData["SuccessMessage"] = $"Order {order.OrderNumber} placed successfully!";
                return RedirectToAction(nameof(Details), new { id = order.Id });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(vm);
            }
        }
    }
}
