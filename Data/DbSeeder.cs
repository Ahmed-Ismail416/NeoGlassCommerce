using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NeoGlassCommerce.Models;

namespace NeoGlassCommerce.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Apply pending migrations
            await context.Database.MigrateAsync();

            // Seed Roles
            string[] roles = { "Admin", "Customer" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Seed Admin User
            var adminEmail = "admin@store.com";
            var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
            if (existingAdmin == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Store Administrator",
                    EmailConfirmed = true
                };
                var createResult = await userManager.CreateAsync(admin, "Password123!");
                if (createResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                    Console.WriteLine("✅ Admin user created successfully: admin@store.com / Password123!");
                }
                else
                {
                    Console.WriteLine("❌ Failed to create admin user:");
                    foreach (var error in createResult.Errors)
                        Console.WriteLine($"   - {error.Code}: {error.Description}");
                }
            }
            else
            {
                // Ensure admin has correct password and role
                var token = await userManager.GeneratePasswordResetTokenAsync(existingAdmin);
                var resetResult = await userManager.ResetPasswordAsync(existingAdmin, token, "Password123!");
                if (resetResult.Succeeded)
                    Console.WriteLine("✅ Admin password reset successfully.");
                else
                {
                    Console.WriteLine("❌ Failed to reset admin password:");
                    foreach (var error in resetResult.Errors)
                        Console.WriteLine($"   - {error.Code}: {error.Description}");
                }
                if (!await userManager.IsInRoleAsync(existingAdmin, "Admin"))
                    await userManager.AddToRoleAsync(existingAdmin, "Admin");
            }

            // Seed Categories
            if (!await context.Categories.AnyAsync())
            {
                var categories = new List<Category>
                {
                    new() { Name = "Electronics" },
                    new() { Name = "Clothing" },
                    new() { Name = "Home & Kitchen" },
                    new() { Name = "Sports & Outdoors" },
                    new() { Name = "Books & Media" }
                };

                context.Categories.AddRange(categories);
                await context.SaveChangesAsync();
            }

            // Seed Products
            if (!await context.Products.AnyAsync())
            {
                var categories = await context.Categories.ToListAsync();
                var electronics = categories.First(c => c.Name == "Electronics").Id;
                var clothing = categories.First(c => c.Name == "Clothing").Id;
                var home = categories.First(c => c.Name == "Home & Kitchen").Id;
                var sports = categories.First(c => c.Name == "Sports & Outdoors").Id;
                var books = categories.First(c => c.Name == "Books & Media").Id;

                var products = new List<Product>
                {
                    // Electronics
                    new() { Name = "Wireless Bluetooth Headphones", SKU = "ELEC-001", Description = "Premium noise-cancelling wireless headphones with 30-hour battery life and deep bass. Perfect for music lovers and professionals.", Price = 79.99m, StockQuantity = 50, CategoryId = electronics, ImageUrl = "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=400", IsActive = true, CreatedAt = DateTime.UtcNow },
                    new() { Name = "Smart Watch Pro", SKU = "ELEC-002", Description = "Advanced fitness tracker with heart rate monitor, GPS, and AMOLED display. Water-resistant up to 50m.", Price = 199.99m, StockQuantity = 35, CategoryId = electronics, ImageUrl = "https://images.unsplash.com/photo-1523275335684-37898b6baf30?w=400", IsActive = true, CreatedAt = DateTime.UtcNow },
                    new() { Name = "Portable Bluetooth Speaker", SKU = "ELEC-003", Description = "Compact waterproof speaker with 360° surround sound and 12-hour playtime. Take your music anywhere.", Price = 49.99m, StockQuantity = 80, CategoryId = electronics, ImageUrl = "https://images.unsplash.com/photo-1608043152269-423dbba4e7e1?w=400", IsActive = true, CreatedAt = DateTime.UtcNow },
                    new() { Name = "USB-C Hub Adapter", SKU = "ELEC-004", Description = "7-in-1 USB-C hub with HDMI, USB 3.0, SD card reader, and PD charging. Essential for productivity.", Price = 34.99m, StockQuantity = 120, CategoryId = electronics, ImageUrl = "https://images.unsplash.com/photo-1625842268584-8f3296236761?w=400", IsActive = true, CreatedAt = DateTime.UtcNow },

                    // Clothing
                    new() { Name = "Classic Cotton T-Shirt", SKU = "CLTH-001", Description = "Premium 100% organic cotton crew-neck t-shirt. Ultra-soft, pre-shrunk, and available in multiple sizes.", Price = 24.99m, StockQuantity = 200, CategoryId = clothing, ImageUrl = "https://images.unsplash.com/photo-1521572163474-6864f9cf17ab?w=400", IsActive = true, CreatedAt = DateTime.UtcNow },
                    new() { Name = "Slim Fit Denim Jeans", SKU = "CLTH-002", Description = "Modern slim-fit jeans crafted from stretch denim. Comfortable all-day wear with classic 5-pocket styling.", Price = 59.99m, StockQuantity = 150, CategoryId = clothing, ImageUrl = "https://images.unsplash.com/photo-1542272454315-4c01d7abdf4a?w=400", IsActive = true, CreatedAt = DateTime.UtcNow },
                    new() { Name = "Lightweight Running Jacket", SKU = "CLTH-003", Description = "Breathable windproof running jacket with reflective details. Packable design fits in your pocket.", Price = 89.99m, StockQuantity = 60, CategoryId = clothing, ImageUrl = "https://images.unsplash.com/photo-1591047139829-d91aecb6caea?w=400", IsActive = true, CreatedAt = DateTime.UtcNow },
                    new() { Name = "Wool Blend Beanie", SKU = "CLTH-004", Description = "Cozy knitted beanie made from soft merino wool blend. Keep warm in style during cold weather.", Price = 19.99m, StockQuantity = 300, CategoryId = clothing, ImageUrl = "https://images.unsplash.com/photo-1576871337632-b9aef4c17ab9?w=400", IsActive = true, CreatedAt = DateTime.UtcNow },

                    // Home & Kitchen
                    new() { Name = "Ceramic Coffee Mug Set", SKU = "HOME-001", Description = "Set of 4 handcrafted ceramic mugs with modern geometric design. Microwave and dishwasher safe.", Price = 39.99m, StockQuantity = 90, CategoryId = home, ImageUrl = "https://images.unsplash.com/photo-1514228742587-6b1558fcca3d?w=400", IsActive = true, CreatedAt = DateTime.UtcNow },
                    new() { Name = "Bamboo Cutting Board", SKU = "HOME-002", Description = "Eco-friendly bamboo cutting board with juice groove and easy-grip handles. Naturally antimicrobial.", Price = 29.99m, StockQuantity = 75, CategoryId = home, ImageUrl = "https://images.unsplash.com/photo-1594226801341-41427b4e5c22?w=400", IsActive = true, CreatedAt = DateTime.UtcNow },
                    new() { Name = "Scented Soy Candle", SKU = "HOME-003", Description = "Hand-poured soy wax candle with lavender and vanilla. 60-hour burn time in elegant glass jar.", Price = 22.99m, StockQuantity = 200, CategoryId = home, ImageUrl = "https://images.unsplash.com/photo-1602028915047-37269d1a73f7?w=400", IsActive = true, CreatedAt = DateTime.UtcNow },
                    new() { Name = "Stainless Steel Water Bottle", SKU = "HOME-004", Description = "Double-wall vacuum insulated bottle keeps drinks cold 24h or hot 12h. BPA-free, 750ml capacity.", Price = 27.99m, StockQuantity = 150, CategoryId = home, ImageUrl = "https://images.unsplash.com/photo-1602143407151-7111542de6e8?w=400", IsActive = true, CreatedAt = DateTime.UtcNow },

                    // Sports & Outdoors
                    new() { Name = "Yoga Mat Premium", SKU = "SPRT-001", Description = "Extra thick 6mm non-slip yoga mat with alignment lines. Eco-friendly TPE material with carrying strap.", Price = 44.99m, StockQuantity = 100, CategoryId = sports, ImageUrl = "https://images.unsplash.com/photo-1601925260368-ae2f83cf8b7f?w=400", IsActive = true, CreatedAt = DateTime.UtcNow },
                    new() { Name = "Resistance Bands Set", SKU = "SPRT-002", Description = "5-piece resistance band set with different tension levels. Includes door anchor and exercise guide.", Price = 19.99m, StockQuantity = 180, CategoryId = sports, ImageUrl = "https://images.unsplash.com/photo-1598289431512-b97b0917affc?w=400", IsActive = true, CreatedAt = DateTime.UtcNow },
                    new() { Name = "Hiking Backpack 40L", SKU = "SPRT-003", Description = "Durable 40L hiking backpack with rain cover, hydration system compatible, and ergonomic design.", Price = 69.99m, StockQuantity = 45, CategoryId = sports, ImageUrl = "https://images.unsplash.com/photo-1553062407-98eeb64c6a62?w=400", IsActive = true, CreatedAt = DateTime.UtcNow },
                    new() { Name = "Adjustable Dumbbell Set", SKU = "SPRT-004", Description = "Adjustable dumbbell pair from 5-25 lbs each. Space-saving design replaces 10 pairs of dumbbells.", Price = 149.99m, StockQuantity = 30, CategoryId = sports, ImageUrl = "https://images.unsplash.com/photo-1586401100295-7a8096fd231a?w=400", IsActive = true, CreatedAt = DateTime.UtcNow },

                    // Books & Media
                    new() { Name = "Design Patterns Handbook", SKU = "BOOK-001", Description = "Comprehensive guide to software design patterns with real-world examples. Essential for developers.", Price = 34.99m, StockQuantity = 60, CategoryId = books, ImageUrl = "https://images.unsplash.com/photo-1532012197267-da84d127e765?w=400", IsActive = true, CreatedAt = DateTime.UtcNow },
                    new() { Name = "Creative Photography Guide", SKU = "BOOK-002", Description = "Master the art of photography with this visual guide. From composition to post-processing techniques.", Price = 29.99m, StockQuantity = 40, CategoryId = books, ImageUrl = "https://images.unsplash.com/photo-1544947950-fa07a98d237f?w=400", IsActive = true, CreatedAt = DateTime.UtcNow },
                    new() { Name = "Mindfulness Journal", SKU = "BOOK-003", Description = "Guided mindfulness journal with daily prompts and gratitude exercises. Beautiful hardcover edition.", Price = 16.99m, StockQuantity = 250, CategoryId = books, ImageUrl = "https://images.unsplash.com/photo-1531346878377-a5be20888e57?w=400", IsActive = true, CreatedAt = DateTime.UtcNow },
                    new() { Name = "World Atlas Collection", SKU = "BOOK-004", Description = "Stunning illustrated world atlas with detailed maps, cultural insights, and geographical data.", Price = 42.99m, StockQuantity = 35, CategoryId = books, ImageUrl = "https://images.unsplash.com/photo-1524578271613-d550eacf6090?w=400", IsActive = true, CreatedAt = DateTime.UtcNow }
                };

                context.Products.AddRange(products);
                await context.SaveChangesAsync();
            }
        }
    }
}
