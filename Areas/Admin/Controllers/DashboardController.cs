using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NeoGlassCommerce.Repositories;
using NeoGlassCommerce.ViewModels;

namespace NeoGlassCommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly IProductRepository _productRepo;
        private readonly IOrderRepository _orderRepo;

        public DashboardController(IProductRepository productRepo, IOrderRepository orderRepo)
        {
            _productRepo = productRepo;
            _orderRepo = orderRepo;
        }

        public async Task<IActionResult> Index()
        {
            var recentOrders = await _orderRepo.GetRecentOrdersAsync(5);

            var vm = new AdminDashboardVM
            {
                TotalProducts = await _productRepo.GetCountAsync(),
                TotalOrders = await _orderRepo.GetCountAsync(),
                TotalRevenue = await _orderRepo.GetTotalRevenueAsync(),
                RecentOrders = recentOrders.Select(o => new OrderListItemVM
                {
                    Id = o.Id,
                    OrderNumber = o.OrderNumber,
                    OrderDate = o.OrderDate,
                    Status = o.Status,
                    TotalAmount = o.TotalAmount,
                    ItemCount = o.OrderItems?.Count ?? 0
                }).ToList()
            };

            return View(vm);
        }
    }
}
