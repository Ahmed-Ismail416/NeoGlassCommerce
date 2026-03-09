using NeoGlassCommerce.Models;

namespace NeoGlassCommerce.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId);
        Task<(IEnumerable<Product> Products, int TotalCount)> SearchAsync(
            int? categoryId, string? query, string? sortBy, int page, int pageSize);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);
        Task<int> GetCountAsync();
    }
}
