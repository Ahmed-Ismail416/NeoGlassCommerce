using Microsoft.AspNetCore.Http;
using NeoGlassCommerce.Repositories;
using NeoGlassCommerce.ViewModels;
using System.Text.Json;

namespace NeoGlassCommerce.Services
{
    public class CartService
    {
        private const string CartSessionKey = "ShoppingCart";
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IProductRepository _productRepo;

        public CartService(IHttpContextAccessor httpContextAccessor, IProductRepository productRepo)
        {
            _httpContextAccessor = httpContextAccessor;
            _productRepo = productRepo;
        }

        private ISession Session => _httpContextAccessor.HttpContext!.Session;

        public CartVM GetCart()
        {
            var cartJson = Session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(cartJson))
                return new CartVM();

            return JsonSerializer.Deserialize<CartVM>(cartJson) ?? new CartVM();
        }

        private void SaveCart(CartVM cart)
        {
            Session.SetString(CartSessionKey, JsonSerializer.Serialize(cart));
        }

        public async Task AddToCartAsync(int productId, int quantity = 1)
        {
            var product = await _productRepo.GetByIdAsync(productId);
            if (product == null || !product.IsActive) return;

            var cart = GetCart();
            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.Items.Add(new CartItemVM
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ImageUrl = product.ImageUrl,
                    UnitPrice = product.Price,
                    Quantity = quantity
                });
            }

            SaveCart(cart);
        }

        public void UpdateQuantity(int productId, int quantity)
        {
            var cart = GetCart();
            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);

            if (item != null)
            {
                if (quantity <= 0)
                    cart.Items.Remove(item);
                else
                    item.Quantity = quantity;
            }

            SaveCart(cart);
        }

        public void RemoveFromCart(int productId)
        {
            var cart = GetCart();
            cart.Items.RemoveAll(i => i.ProductId == productId);
            SaveCart(cart);
        }

        public void ClearCart()
        {
            Session.Remove(CartSessionKey);
        }

        public int GetCartItemCount()
        {
            return GetCart().ItemCount;
        }
    }
}
