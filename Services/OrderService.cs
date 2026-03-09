using Microsoft.EntityFrameworkCore;
using NeoGlassCommerce.Data;
using NeoGlassCommerce.Models;
using NeoGlassCommerce.Repositories;
using NeoGlassCommerce.ViewModels;

namespace NeoGlassCommerce.Services
{
    public class OrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IProductRepository _productRepo;
        private readonly CartService _cartService;

        public OrderService(
            ApplicationDbContext context,
            IProductRepository productRepo,
            CartService cartService)
        {
            _context = context;
            _productRepo = productRepo;
            _cartService = cartService;
        }

        public async Task<Order> PlaceOrderAsync(string userId, CheckoutVM checkout)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var cart = _cartService.GetCart();
                if (!cart.Items.Any())
                    throw new InvalidOperationException("Cart is empty.");

                // 1. Validate stock availability
                foreach (var item in cart.Items)
                {
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product == null || !product.IsActive)
                        throw new InvalidOperationException($"Product '{item.ProductName}' is no longer available.");
                    if (product.StockQuantity < item.Quantity)
                        throw new InvalidOperationException($"Insufficient stock for '{product.Name}'. Available: {product.StockQuantity}");
                }

                // 2. Create or get shipping address
                var address = new Address
                {
                    UserId = userId,
                    Country = checkout.Country,
                    City = checkout.City,
                    Street = checkout.Street,
                    Zip = checkout.Zip,
                    IsDefault = false
                };
                _context.Addresses.Add(address);
                await _context.SaveChangesAsync();

                // 3. Create Order
                var order = new Order
                {
                    UserId = userId,
                    ShippingAddressId = address.Id,
                    OrderNumber = GenerateOrderNumber(),
                    Status = OrderStatus.Pending,
                    OrderDate = DateTime.UtcNow,
                    TotalAmount = cart.Total
                };
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // 4. Create OrderItems & Decrease stock
                foreach (var item in cart.Items)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = order.Id,
                        ProductId = item.ProductId,
                        UnitPrice = item.UnitPrice,
                        Quantity = item.Quantity,
                        LineTotal = item.LineTotal
                    };
                    _context.OrderItems.Add(orderItem);

                    var product = await _context.Products.FindAsync(item.ProductId);
                    product!.StockQuantity -= item.Quantity;
                }

                // 5. Save all & commit
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Clear cart
                _cartService.ClearCart();

                return order;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<OrderListItemVM>> GetUserOrdersAsync(string userId)
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return orders.Select(o => new OrderListItemVM
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                OrderDate = o.OrderDate,
                Status = o.Status,
                TotalAmount = o.TotalAmount,
                ItemCount = o.OrderItems.Count
            }).ToList();
        }

        public async Task<OrderDetailsVM?> GetOrderDetailsAsync(int orderId, string userId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                .Include(o => o.ShippingAddress)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

            if (order == null) return null;

            return MapToOrderDetailsVM(order);
        }

        public async Task<OrderDetailsVM?> GetOrderDetailsAdminAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                .Include(o => o.ShippingAddress)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null) return null;

            return MapToOrderDetailsVM(order);
        }

        private OrderDetailsVM MapToOrderDetailsVM(Order order)
        {
            return new OrderDetailsVM
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                OrderDate = order.OrderDate,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                ShippingCountry = order.ShippingAddress?.Country ?? "",
                ShippingCity = order.ShippingAddress?.City ?? "",
                ShippingStreet = order.ShippingAddress?.Street ?? "",
                ShippingZip = order.ShippingAddress?.Zip ?? "",
                Items = order.OrderItems.Select(oi => new OrderItemVM
                {
                    ProductName = oi.Product?.Name ?? "",
                    ImageUrl = oi.Product?.ImageUrl ?? "",
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity,
                    LineTotal = oi.LineTotal
                }).ToList()
            };
        }

        private static string GenerateOrderNumber()
        {
            return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
        }
    }
}
