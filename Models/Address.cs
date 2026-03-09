using System.ComponentModel.DataAnnotations;

namespace NeoGlassCommerce.Models
{
    public class Address
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Country { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [Required, MaxLength(300)]
        public string Street { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string Zip { get; set; } = string.Empty;

        public bool IsDefault { get; set; }

        public ApplicationUser User { get; set; } = null!;

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
