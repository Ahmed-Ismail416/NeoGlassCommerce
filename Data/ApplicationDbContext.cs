using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NeoGlassCommerce.Models;

namespace NeoGlassCommerce.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<Address> Addresses => Set<Address>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Category self-referencing
            builder.Entity<Category>(e =>
            {
                e.HasOne(c => c.ParentCategory)
                 .WithMany(c => c.SubCategories)
                 .HasForeignKey(c => c.ParentCategoryId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(c => c.Name).IsUnique();
            });

            // Product -> Category
            builder.Entity<Product>(e =>
            {
                e.HasOne(p => p.Category)
                 .WithMany(c => c.Products)
                 .HasForeignKey(p => p.CategoryId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(p => p.SKU).IsUnique();
            });

            // Order -> User
            builder.Entity<Order>(e =>
            {
                e.HasOne(o => o.User)
                 .WithMany(u => u.Orders)
                 .HasForeignKey(o => o.UserId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(o => o.ShippingAddress)
                 .WithMany(a => a.Orders)
                 .HasForeignKey(o => o.ShippingAddressId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(o => o.OrderNumber).IsUnique();
            });

            // OrderItem -> Order & Product
            builder.Entity<OrderItem>(e =>
            {
                e.HasOne(oi => oi.Order)
                 .WithMany(o => o.OrderItems)
                 .HasForeignKey(oi => oi.OrderId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(oi => oi.Product)
                 .WithMany(p => p.OrderItems)
                 .HasForeignKey(oi => oi.ProductId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // Address -> User
            builder.Entity<Address>(e =>
            {
                e.HasOne(a => a.User)
                 .WithMany(u => u.Addresses)
                 .HasForeignKey(a => a.UserId)
                 .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
