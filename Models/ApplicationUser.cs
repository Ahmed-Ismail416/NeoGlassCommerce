using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace NeoGlassCommerce.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required, MaxLength(200)]
        public string FullName { get; set; } = string.Empty;

        public ICollection<Order> Orders { get; set; } = new List<Order>();

        public ICollection<Address> Addresses { get; set; } = new List<Address>();
    }
}
