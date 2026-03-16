using Microsoft.EntityFrameworkCore;
using NeoGlassCommerce.Data;
using NeoGlassCommerce.Models;

namespace NeoGlassCommerce.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == categoryId && p.IsActive)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Product> Products, int TotalCount)> SearchAsync(
            int? categoryId, string? query, string? sortBy, int page, int pageSize)
        {
            var q = _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive)
                .AsQueryable();

            if (categoryId.HasValue)
                q = q.Where(p => p.CategoryId == categoryId.Value);

            if (!string.IsNullOrWhiteSpace(query))
                q = q.Where(p => p.Name.Contains(query) || p.Description.Contains(query));

            var totalCount = await q.CountAsync();

            q = sortBy switch
            {
                "price_asc" => q.OrderBy(p => p.Price),
                "price_desc" => q.OrderByDescending(p => p.Price),
                "name" => q.OrderBy(p => p.Name),
                "newest" => q.OrderByDescending(p => p.CreatedAt),
                _ => q.OrderByDescending(p => p.CreatedAt)
            };

            var products = await q
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (products, totalCount);
        }

        public async Task AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                product.IsActive = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> HasOrdersAsync(int id)
        {
            return await _context.OrderItems.AnyAsync(oi => oi.ProductId == id);
        }

        public async Task<bool> ExistsWithSKUAsync(string sku, int? excludeId = null)
        {
            return await _context.Products.AnyAsync(p =>
                p.SKU == sku && (excludeId == null || p.Id != excludeId));
        }

        public async Task<int> GetCountAsync()
        {
            return await _context.Products.CountAsync(p => p.IsActive);
        }
    }
}
