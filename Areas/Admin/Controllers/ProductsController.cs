using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NeoGlassCommerce.Models;
using NeoGlassCommerce.Repositories;
using NeoGlassCommerce.ViewModels;

namespace NeoGlassCommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly IProductRepository _productRepo;
        private readonly ICategoryRepository _categoryRepo;

        public ProductsController(IProductRepository productRepo, ICategoryRepository categoryRepo)
        {
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productRepo.GetAllAsync();
            var vm = products.Select(p => new AdminProductVM
            {
                Id = p.Id,
                Name = p.Name,
                SKU = p.SKU,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                CategoryName = p.Category?.Name,
                ImageUrl = p.ImageUrl,
                IsActive = p.IsActive
            }).ToList();

            return View(vm);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateCategoriesAsync();
            return View(new AdminProductVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminProductVM vm)
        {
            if (!ModelState.IsValid)
            {
                await PopulateCategoriesAsync();
                return View(vm);
            }

            var product = new Product
            {
                Name = vm.Name,
                SKU = vm.SKU,
                Description = vm.Description,
                Price = vm.Price,
                StockQuantity = vm.StockQuantity,
                CategoryId = vm.CategoryId,
                ImageUrl = vm.ImageUrl,
                IsActive = vm.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            await _productRepo.AddAsync(product);
            TempData["Success"] = "Product created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null) return NotFound();

            await PopulateCategoriesAsync();

            var vm = new AdminProductVM
            {
                Id = product.Id,
                Name = product.Name,
                SKU = product.SKU,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                CategoryId = product.CategoryId,
                ImageUrl = product.ImageUrl,
                IsActive = product.IsActive
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AdminProductVM vm)
        {
            if (!ModelState.IsValid)
            {
                await PopulateCategoriesAsync();
                return View(vm);
            }

            var product = await _productRepo.GetByIdAsync(vm.Id);
            if (product == null) return NotFound();

            product.Name = vm.Name;
            product.SKU = vm.SKU;
            product.Description = vm.Description;
            product.Price = vm.Price;
            product.StockQuantity = vm.StockQuantity;
            product.CategoryId = vm.CategoryId;
            product.ImageUrl = vm.ImageUrl;
            product.IsActive = vm.IsActive;

            await _productRepo.UpdateAsync(product);
            TempData["Success"] = "Product updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _productRepo.DeleteAsync(id);
            TempData["Success"] = "Product deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateCategoriesAsync()
        {
            var categories = await _categoryRepo.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
        }
    }
}
