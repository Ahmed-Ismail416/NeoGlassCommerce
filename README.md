```markdown
# NeoGlass Commerce

A modern, full-featured **ASP.NET Core MVC E-Commerce** application with a stunning **Glassmorphism UI** design. Built with .NET 8, Entity Framework Core, and ASP.NET Core Identity.

![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet)
![ASP.NET Core MVC](https://img.shields.io/badge/ASP.NET%20Core-MVC-512BD4?style=flat-square)
![TailwindCSS](https://img.shields.io/badge/Tailwind-CSS-06B6D4?style=flat-square&logo=tailwindcss)
![SQL Server](https://img.shields.io/badge/SQL-Server-CC2927?style=flat-square&logo=microsoftsqlserver)

---

## Features

### Customer Features

- **Product Catalog** — Browse products with category filtering, search, sorting & pagination
- **Product Details** — View detailed product info with quantity selector
- **Shopping Cart** — Session-based cart with add/update/remove functionality
- **Checkout** — Transactional order placement with address form
- **Order History** — View all past orders with status tracking
- **User Authentication** — Register & login with ASP.NET Core Identity

### Admin Panel

- **Dashboard** — Overview cards (Total Products, Orders, Revenue) + Recent Orders
- **Products CRUD** — Create, Edit, Delete products
- **Categories CRUD** — Create, Edit, Delete categories
- **Orders Management** — View all orders and update order status

---

## Architecture

The project follows a **layered architecture** with clean separation of concerns:
```

NeoGlassCommerce/
├── Areas/
│ └── Admin/
│ ├── Controllers/ # Admin controllers (Dashboard, Products, Categories, Orders)
│ └── Views/ # Admin Razor views with sidebar layout
├── Controllers/ # Public controllers (Catalog, Cart, Orders, Account)
├── Data/
│ ├── ApplicationDbContext.cs # EF Core DbContext with Fluent API
│ └── DbSeeder.cs # Database seed data
├── Models/ # Entity models (Product, Category, Order, etc.)
├── ViewModels/ # View-specific DTOs (no EF entities in views)
├── Repositories/ # Repository pattern interfaces & implementations
├── Services/ # Business logic (ProductService, CartService, OrderService)
├── Views/
│ ├── Catalog/ # Product listing & details
│ ├── Cart/ # Shopping cart
│ ├── Orders/ # Checkout & order history
│ ├── Account/ # Login & Register
│ └── Shared/ # Layout, Navbar, ProductCard, Pagination, CartSummary
└── wwwroot/ # Static files

````

---

## Database Schema

| Entity              | Key Fields                                                                   |
| ------------------- | ---------------------------------------------------------------------------- |
| **ApplicationUser** | FullName (extends IdentityUser)                                              |
| **Category**        | Name, ParentCategoryId (self-referencing)                                    |
| **Product**         | Name, SKU, Description, Price, StockQuantity, CategoryId, ImageUrl, IsActive |
| **Order**           | UserId, ShippingAddressId, OrderNumber, Status, TotalAmount                  |
| **OrderItem**       | OrderId, ProductId, UnitPrice, Quantity, LineTotal                           |
| **Address**         | UserId, Country, City, Street, Zip, IsDefault                                |

### Relationships

- Category `1..*` Product
- User `1..*` Order
- Order `1..*` OrderItem
- Product `1..*` OrderItem
- User `1..*` Address
- Order → ShippingAddress

---

## Tech Stack

| Layer              | Technology                           |
| ------------------ | ------------------------------------ |
| **Framework**      | ASP.NET Core 8 MVC                   |
| **ORM**            | Entity Framework Core 8 (Code First) |
| **Database**       | SQL Server (LocalDB)                 |
| **Authentication** | ASP.NET Core Identity                |
| **Frontend CSS**   | TailwindCSS (CDN)                    |
| **Frontend JS**    | Alpine.js (CDN)                      |
| **UI Design**      | Glassmorphism (NeoGlass theme)       |
| **Font**           | Poppins (Google Fonts)               |

---

## UI Design — NeoGlass Theme

| Element              | Value                                                   |
| -------------------- | ------------------------------------------------------- |
| **Primary Gradient** | `#6366F1` → `#8B5CF6` (Indigo → Violet)                 |
| **Background**       | `#F1F5F9` (Light Slate)                                 |
| **Accent**           | `#22C55E` (Green)                                       |
| **Danger**           | `#EF4444` (Red)                                         |
| **Cards**            | Glassmorphism with backdrop blur, rounded-xl, shadow-lg |
| **Font**             | Poppins (300–700 weights)                               |

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/sql-server) (LocalDB or full instance)

### Setup & Run

1. **Clone the repository**

```bash
git clone <repository-url>
cd NeoGlassCommerce
````

2. **Update the connection string** in `appsettings.json` if needed:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=NeoGlassCommerceDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
}
```

3. **Run the application** (migrations & seeding are automatic):

```bash
dotnet run
```

4. **Open in browser**

```
https://localhost:5001
```

The database, migrations, roles, admin user, categories, and 20 sample products are all seeded automatically on first run.

---

## Default Accounts

| Role      | Email             | Password       |
| --------- | ----------------- | -------------- |
| **Admin** | `admin@store.com` | `Password123!` |

You can register new Customer accounts from the UI.

---

## Checkout Business Logic

The checkout process is fully transactional:

1. Validate stock availability for all cart items
2. Create shipping address
3. Create Order record
4. Create OrderItem records for each cart item
5. Decrease product stock quantities
6. Save all changes in a single database transaction
7. Clear the shopping cart on success
8. Rollback everything on any failure

---

## Key Design Patterns

| Pattern                  | Usage                                                           |
| ------------------------ | --------------------------------------------------------------- |
| **Repository Pattern**   | `IProductRepository`, `ICategoryRepository`, `IOrderRepository` |
| **Service Layer**        | `ProductService`, `CartService`, `OrderService`                 |
| **ViewModel Pattern**    | No EF entities passed directly to views                         |
| **Dependency Injection** | All services & repositories registered in DI container          |
| **Session-based Cart**   | Shopping cart stored in server-side session                     |
| **Areas**                | Admin panel separated using ASP.NET Core Areas                  |

---

## Seed Data

The application seeds the following on first run:

- **2 Roles**: Admin, Customer
- **1 Admin User**: `admin@store.com`
- **5 Categories**: Electronics, Clothing, Home & Kitchen, Sports & Outdoors, Books & Media
- **20 Products**: 4 products per category with real Unsplash images

---

## Pages Overview

| Page             | Route                   | Description                                 |
| ---------------- | ----------------------- | ------------------------------------------- |
| Product Catalog  | `/Catalog`              | Grid with filters, search, sort, pagination |
| Product Details  | `/Catalog/Details/{id}` | Full product view with add-to-cart          |
| Shopping Cart    | `/Cart`                 | Cart items with quantity controls           |
| Checkout         | `/Orders/Checkout`      | Address form + order summary                |
| My Orders        | `/Orders`               | Customer order history                      |
| Order Details    | `/Orders/Details/{id}`  | Single order breakdown                      |
| Login            | `/Account/Login`        | User authentication                         |
| Register         | `/Account/Register`     | New account creation                        |
| Admin Dashboard  | `/Admin/Dashboard`      | Stats + recent orders                       |
| Admin Products   | `/Admin/Products`       | CRUD table                                  |
| Admin Categories | `/Admin/Categories`     | CRUD table                                  |
| Admin Orders     | `/Admin/Orders`         | Order management                            |

---

## Author

Built as an academic E-Commerce project using **ASP.NET Core 8 MVC** with modern web design principles.

---

## License

This project is for educational purposes. Feel free to use and modify.
