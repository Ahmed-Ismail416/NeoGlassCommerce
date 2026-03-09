using System.ComponentModel.DataAnnotations;

namespace NeoGlassCommerce.ViewModels
{
    public class CheckoutVM
    {
        [Required, MaxLength(100)]
        public string Country { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [Required, MaxLength(300)]
        public string Street { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string Zip { get; set; } = string.Empty;

        public CartVM Cart { get; set; } = new();
    }
}
