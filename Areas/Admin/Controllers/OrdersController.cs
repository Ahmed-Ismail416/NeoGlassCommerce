using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NeoGlassCommerce.Models;
using NeoGlassCommerce.Repositories;
using NeoGlassCommerce.Services;
using NeoGlassCommerce.ViewModels;

namespace NeoGlassCommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrdersController : Controller
    {
        private readonly IOrderRepository _orderRepo;
        private readonly OrderService _orderService;

        public OrdersController(IOrderRepository orderRepo, OrderService orderService)
        {
            _orderRepo = orderRepo;
            _orderService = orderService;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _orderRepo.GetAllAsync();
            var vm = orders.Select(o => new AdminOrderVM
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                CustomerName = o.User?.FullName ?? o.User?.Email ?? "",
                OrderDate = o.OrderDate,
                Status = o.Status,
                TotalAmount = o.TotalAmount
            }).ToList();

            return View(vm);
        }

        public async Task<IActionResult> Details(int id)
        {
            var vm = await _orderService.GetOrderDetailsAdminAsync(id);
            if (vm == null) return NotFound();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, OrderStatus status)
        {
            var order = await _orderRepo.GetByIdAsync(id);
            if (order == null) return NotFound();

            order.Status = status;
            await _orderRepo.UpdateAsync(order);

            TempData["Success"] = "Order status updated.";
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
