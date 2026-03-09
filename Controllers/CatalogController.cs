using Microsoft.AspNetCore.Mvc;
using NeoGlassCommerce.Services;

namespace NeoGlassCommerce.Controllers
{
    public class CatalogController : Controller
    {
        private readonly ProductService _productService;

        public CatalogController(ProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index(int? categoryId, string? q, string? sort, int page = 1)
        {
            var vm = await _productService.GetProductListAsync(categoryId, q, sort, page);
            return View(vm);
        }

        public async Task<IActionResult> Details(int id)
        {
            var vm = await _productService.GetProductDetailsAsync(id);
            if (vm == null) return NotFound();
            return View(vm);
        }
    }
}
