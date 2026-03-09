namespace NeoGlassCommerce.ViewModels
{
    public class CartItemVM
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal => UnitPrice * Quantity;
    }

    public class CartVM
    {
        public List<CartItemVM> Items { get; set; } = new();
        public decimal Total => Items.Sum(i => i.LineTotal);
        public int ItemCount => Items.Sum(i => i.Quantity);
    }
}
