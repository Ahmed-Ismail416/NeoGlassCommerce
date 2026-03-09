using NeoGlassCommerce.Models;

namespace NeoGlassCommerce.Repositories
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllAsync();
        Task<IEnumerable<Order>> GetByUserIdAsync(string userId);
        Task<Order?> GetByIdAsync(int id);
        Task AddAsync(Order order);
        Task UpdateAsync(Order order);
        Task<int> GetCountAsync();
        Task<decimal> GetTotalRevenueAsync();
        Task<IEnumerable<Order>> GetRecentOrdersAsync(int count);
    }
}
