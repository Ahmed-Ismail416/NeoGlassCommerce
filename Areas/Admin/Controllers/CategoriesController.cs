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
    public class CategoriesController : Controller
    {
        private readonly ICategoryRepository _categoryRepo;

        public CategoriesController(ICategoryRepository categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _categoryRepo.GetAllAsync();
            var vm = categories.Select(c => new AdminCategoryVM
            {
                Id = c.Id,
                Name = c.Name,
                ParentCategoryId = c.ParentCategoryId,
                ParentCategoryName = c.ParentCategory?.Name,
                ProductCount = c.Products?.Count ?? 0
            }).ToList();

            return View(vm);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateParentsAsync();
            return View(new AdminCategoryVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminCategoryVM vm)
        {
            if (!ModelState.IsValid)
            {
                await PopulateParentsAsync();
                return View(vm);
            }

            var category = new Category
            {
                Name = vm.Name,
                ParentCategoryId = vm.ParentCategoryId
            };

            await _categoryRepo.AddAsync(category);
            TempData["Success"] = "Category created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryRepo.GetByIdAsync(id);
            if (category == null) return NotFound();

            await PopulateParentsAsync();

            var vm = new AdminCategoryVM
            {
                Id = category.Id,
                Name = category.Name,
                ParentCategoryId = category.ParentCategoryId
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AdminCategoryVM vm)
        {
            if (!ModelState.IsValid)
            {
                await PopulateParentsAsync();
                return View(vm);
            }

            var category = await _categoryRepo.GetByIdAsync(vm.Id);
            if (category == null) return NotFound();

            category.Name = vm.Name;
            category.ParentCategoryId = vm.ParentCategoryId;

            await _categoryRepo.UpdateAsync(category);
            TempData["Success"] = "Category updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _categoryRepo.DeleteAsync(id);
                TempData["Success"] = "Category deleted successfully.";
            }
            catch
            {
                TempData["Error"] = "Cannot delete category with products.";
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateParentsAsync()
        {
            var categories = await _categoryRepo.GetAllAsync();
            ViewBag.ParentCategories = new SelectList(categories, "Id", "Name");
        }
    }
}
