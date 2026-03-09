using NeoGlassCommerce.Models;
using NeoGlassCommerce.Repositories;
using NeoGlassCommerce.ViewModels;

namespace NeoGlassCommerce.Services
{
    public class ProductService
    {
        private readonly IProductRepository _productRepo;
        private readonly ICategoryRepository _categoryRepo;

        public ProductService(IProductRepository productRepo, ICategoryRepository categoryRepo)
        {
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
        }

        public async Task<ProductListVM> GetProductListAsync(
            int? categoryId, string? query, string? sortBy, int page, int pageSize = 8)
        {
            var (products, totalCount) = await _productRepo.SearchAsync(categoryId, query, sortBy, page, pageSize);
            var categories = await _categoryRepo.GetAllAsync();

            return new ProductListVM
            {
                Products = products.Select(p => new ProductCardVM
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    StockQuantity = p.StockQuantity,
                    CategoryName = p.Category?.Name ?? ""
                }).ToList(),
                Categories = categories.Select(c => new CategoryFilterVM
                {
                    Id = c.Id,
                    Name = c.Name,
                    ProductCount = c.Products?.Count(p => p.IsActive) ?? 0
                }).ToList(),
                SelectedCategoryId = categoryId,
                SearchQuery = query,
                SortBy = sortBy,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                TotalItems = totalCount
            };
        }

        public async Task<ProductDetailsVM?> GetProductDetailsAsync(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null) return null;

            return new ProductDetailsVM
            {
                Id = product.Id,
                Name = product.Name,
                SKU = product.SKU,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                ImageUrl = product.ImageUrl,
                CategoryName = product.Category?.Name ?? "",
                IsActive = product.IsActive,
                CreatedAt = product.CreatedAt
            };
        }
    }
}
