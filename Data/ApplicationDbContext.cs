namespace COMP2139_assign01.Data;
using COMP2139_assign01.Models;
using Microsoft.EntityFrameworkCore;
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options){ }

    public DbSet<Inventory> Inventory { get; set; }

    public DbSet<InventoryTask> Tasks { get; set; }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    public DbSet<ApplicationUserCategory> UserCategories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Seed Categories
        var categories = new[]
        {
            new Category { CategoryId = 1, Name = "Electronics", Description = "Electronic devices and accessories" },
            new Category { CategoryId = 2, Name = "Books", Description = "Books and educational materials" },
            new Category { CategoryId = 3, Name = "Clothing", Description = "Apparel and fashion items" },
            new Category { CategoryId = 4, Name = "Home & Kitchen", Description = "Home and kitchen appliances" },
            new Category { CategoryId = 5, Name = "Sports & Outdoors", Description = "Sports equipment and outdoor gear" },
            new Category { CategoryId = 6, Name = "Toys & Games", Description = "Toys, games, and entertainment items" },
            new Category { CategoryId = 7, Name = "Beauty & Personal Care", Description = "Beauty and personal care products" },
            new Category { CategoryId = 8, Name = "Office Supplies", Description = "Office and stationery items" },
            new Category { CategoryId = 9, Name = "Pet Supplies", Description = "Pet food and accessories" },
            new Category { CategoryId = 10, Name = "Automotive", Description = "Car accessories and maintenance items" }
        };

        modelBuilder.Entity<Category>().HasData(categories);

        // Seed Products
        var random = new Random(123); // Fixed seed for consistent data
        var products = new List<Product>();

        // Electronics
        products.AddRange(new[]
        {
            new Product { ProductId = 1, Name = "Wireless Earbuds", Description = "High-quality wireless earbuds with noise cancellation", SKU = "ELEC-001", CategoryId = 1, Price = 79.99m, Quantity = random.Next(5, 50), LowStockThreshold = 10 },
            new Product { ProductId = 2, Name = "Smart Watch", Description = "Feature-rich smartwatch with health tracking", SKU = "ELEC-002", CategoryId = 1, Price = 199.99m, Quantity = random.Next(5, 50), LowStockThreshold = 8 },
            new Product { ProductId = 3, Name = "Bluetooth Speaker", Description = "Portable Bluetooth speaker with great sound", SKU = "ELEC-003", CategoryId = 1, Price = 49.99m, Quantity = random.Next(5, 50), LowStockThreshold = 12 },
            new Product { ProductId = 4, Name = "USB-C Charger", Description = "Fast-charging USB-C power adapter", SKU = "ELEC-004", CategoryId = 1, Price = 24.99m, Quantity = random.Next(5, 50), LowStockThreshold = 15 },
            new Product { ProductId = 5, Name = "Wireless Mouse", Description = "Ergonomic wireless mouse", SKU = "ELEC-005", CategoryId = 1, Price = 29.99m, Quantity = random.Next(5, 50), LowStockThreshold = 10 }
        });

        // Books
        products.AddRange(new[]
        {
            new Product { ProductId = 6, Name = "The Art of Programming", Description = "Comprehensive programming guide", SKU = "BOOK-001", CategoryId = 2, Price = 39.99m, Quantity = random.Next(5, 50), LowStockThreshold = 7 },
            new Product { ProductId = 7, Name = "Data Science Basics", Description = "Introduction to data science", SKU = "BOOK-002", CategoryId = 2, Price = 29.99m, Quantity = random.Next(5, 50), LowStockThreshold = 7 },
            new Product { ProductId = 8, Name = "Web Development Guide", Description = "Modern web development techniques", SKU = "BOOK-003", CategoryId = 2, Price = 34.99m, Quantity = random.Next(5, 50), LowStockThreshold = 7 },
            new Product { ProductId = 9, Name = "AI & Machine Learning", Description = "AI and ML fundamentals", SKU = "BOOK-004", CategoryId = 2, Price = 44.99m, Quantity = random.Next(5, 50), LowStockThreshold = 7 },
            new Product { ProductId = 10, Name = "Cloud Computing 101", Description = "Introduction to cloud computing", SKU = "BOOK-005", CategoryId = 2, Price = 32.99m, Quantity = random.Next(5, 50), LowStockThreshold = 7 }
        });

        // Clothing
        products.AddRange(new[]
        {
            new Product { ProductId = 11, Name = "Cotton T-Shirt", Description = "Comfortable cotton t-shirt", SKU = "CLTH-001", CategoryId = 3, Price = 19.99m, Quantity = random.Next(5, 50), LowStockThreshold = 15 },
            new Product { ProductId = 12, Name = "Denim Jeans", Description = "Classic denim jeans", SKU = "CLTH-002", CategoryId = 3, Price = 49.99m, Quantity = random.Next(5, 50), LowStockThreshold = 10 },
            new Product { ProductId = 13, Name = "Hooded Sweatshirt", Description = "Warm hooded sweatshirt", SKU = "CLTH-003", CategoryId = 3, Price = 39.99m, Quantity = random.Next(5, 50), LowStockThreshold = 12 },
            new Product { ProductId = 14, Name = "Running Shoes", Description = "Comfortable running shoes", SKU = "CLTH-004", CategoryId = 3, Price = 89.99m, Quantity = random.Next(5, 50), LowStockThreshold = 8 },
            new Product { ProductId = 15, Name = "Winter Jacket", Description = "Warm winter jacket", SKU = "CLTH-005", CategoryId = 3, Price = 129.99m, Quantity = random.Next(5, 50), LowStockThreshold = 8 }
        });

        // Home & Kitchen
        products.AddRange(new[]
        {
            new Product { ProductId = 16, Name = "Coffee Maker", Description = "Programmable coffee maker", SKU = "HOME-001", CategoryId = 4, Price = 79.99m, Quantity = random.Next(5, 50), LowStockThreshold = 8 },
            new Product { ProductId = 17, Name = "Toaster Oven", Description = "Multi-function toaster oven", SKU = "HOME-002", CategoryId = 4, Price = 69.99m, Quantity = random.Next(5, 50), LowStockThreshold = 8 },
            new Product { ProductId = 18, Name = "Blender", Description = "High-speed blender", SKU = "HOME-003", CategoryId = 4, Price = 49.99m, Quantity = random.Next(5, 50), LowStockThreshold = 10 },
            new Product { ProductId = 19, Name = "Cookware Set", Description = "Complete cookware set", SKU = "HOME-004", CategoryId = 4, Price = 149.99m, Quantity = random.Next(5, 50), LowStockThreshold = 6 },
            new Product { ProductId = 20, Name = "Kitchen Knife Set", Description = "Professional kitchen knife set", SKU = "HOME-005", CategoryId = 4, Price = 89.99m, Quantity = random.Next(5, 50), LowStockThreshold = 7 }
        });

        // Sports & Outdoors
        products.AddRange(new[]
        {
            new Product { ProductId = 21, Name = "Yoga Mat", Description = "Non-slip yoga mat", SKU = "SPRT-001", CategoryId = 5, Price = 24.99m, Quantity = random.Next(5, 50), LowStockThreshold = 12 },
            new Product { ProductId = 22, Name = "Dumbbells Set", Description = "Adjustable dumbbells set", SKU = "SPRT-002", CategoryId = 5, Price = 79.99m, Quantity = random.Next(5, 50), LowStockThreshold = 8 },
            new Product { ProductId = 23, Name = "Tennis Racket", Description = "Professional tennis racket", SKU = "SPRT-003", CategoryId = 5, Price = 59.99m, Quantity = random.Next(5, 50), LowStockThreshold = 8 },
            new Product { ProductId = 24, Name = "Basketball", Description = "Indoor/outdoor basketball", SKU = "SPRT-004", CategoryId = 5, Price = 29.99m, Quantity = random.Next(5, 50), LowStockThreshold = 10 },
            new Product { ProductId = 25, Name = "Camping Tent", Description = "4-person camping tent", SKU = "SPRT-005", CategoryId = 5, Price = 199.99m, Quantity = random.Next(5, 50), LowStockThreshold = 6 }
        });

        // Toys & Games
        products.AddRange(new[]
        {
            new Product { ProductId = 26, Name = "Board Game Set", Description = "Classic board games collection", SKU = "TOYS-001", CategoryId = 6, Price = 34.99m, Quantity = random.Next(5, 50), LowStockThreshold = 10 },
            new Product { ProductId = 27, Name = "Remote Control Car", Description = "High-speed RC car", SKU = "TOYS-002", CategoryId = 6, Price = 44.99m, Quantity = random.Next(5, 50), LowStockThreshold = 8 },
            new Product { ProductId = 28, Name = "Building Blocks", Description = "Creative building blocks set", SKU = "TOYS-003", CategoryId = 6, Price = 29.99m, Quantity = random.Next(5, 50), LowStockThreshold = 12 },
            new Product { ProductId = 29, Name = "Puzzle Set", Description = "1000-piece puzzle set", SKU = "TOYS-004", CategoryId = 6, Price = 19.99m, Quantity = random.Next(5, 50), LowStockThreshold = 15 },
            new Product { ProductId = 30, Name = "Art Supply Kit", Description = "Complete art supply kit", SKU = "TOYS-005", CategoryId = 6, Price = 24.99m, Quantity = random.Next(5, 50), LowStockThreshold = 10 }
        });

        // Beauty & Personal Care
        products.AddRange(new[]
        {
            new Product { ProductId = 31, Name = "Hair Dryer", Description = "Professional hair dryer", SKU = "BEAU-001", CategoryId = 7, Price = 49.99m, Quantity = random.Next(5, 50), LowStockThreshold = 10 },
            new Product { ProductId = 32, Name = "Electric Toothbrush", Description = "Sonic electric toothbrush", SKU = "BEAU-002", CategoryId = 7, Price = 39.99m, Quantity = random.Next(5, 50), LowStockThreshold = 12 },
            new Product { ProductId = 33, Name = "Face Moisturizer", Description = "Hydrating face moisturizer", SKU = "BEAU-003", CategoryId = 7, Price = 24.99m, Quantity = random.Next(5, 50), LowStockThreshold = 15 },
            new Product { ProductId = 34, Name = "Shampoo Set", Description = "Premium shampoo and conditioner", SKU = "BEAU-004", CategoryId = 7, Price = 29.99m, Quantity = random.Next(5, 50), LowStockThreshold = 15 },
            new Product { ProductId = 35, Name = "Makeup Kit", Description = "Professional makeup kit", SKU = "BEAU-005", CategoryId = 7, Price = 59.99m, Quantity = random.Next(5, 50), LowStockThreshold = 8 }
        });

        // Office Supplies
        products.AddRange(new[]
        {
            new Product { ProductId = 36, Name = "Desk Organizer", Description = "Multi-compartment desk organizer", SKU = "OFFC-001", CategoryId = 8, Price = 19.99m, Quantity = random.Next(5, 50), LowStockThreshold = 12 },
            new Product { ProductId = 37, Name = "Notebook Set", Description = "Premium notebook set", SKU = "OFFC-002", CategoryId = 8, Price = 14.99m, Quantity = random.Next(5, 50), LowStockThreshold = 15 },
            new Product { ProductId = 38, Name = "Printer Paper", Description = "High-quality printer paper", SKU = "OFFC-003", CategoryId = 8, Price = 9.99m, Quantity = random.Next(5, 50), LowStockThreshold = 20 },
            new Product { ProductId = 39, Name = "Pen Set", Description = "Luxury pen set", SKU = "OFFC-004", CategoryId = 8, Price = 7.99m, Quantity = random.Next(5, 50), LowStockThreshold = 20 },
            new Product { ProductId = 40, Name = "Stapler", Description = "Heavy-duty stapler", SKU = "OFFC-005", CategoryId = 8, Price = 8.99m, Quantity = random.Next(5, 50), LowStockThreshold = 15 }
        });

        // Pet Supplies
        products.AddRange(new[]
        {
            new Product { ProductId = 41, Name = "Dog Bed", Description = "Comfortable dog bed", SKU = "PETS-001", CategoryId = 9, Price = 39.99m, Quantity = random.Next(5, 50), LowStockThreshold = 8 },
            new Product { ProductId = 42, Name = "Cat Litter Box", Description = "Self-cleaning litter box", SKU = "PETS-002", CategoryId = 9, Price = 29.99m, Quantity = random.Next(5, 50), LowStockThreshold = 10 },
            new Product { ProductId = 43, Name = "Pet Food Bowl", Description = "Stainless steel food bowl", SKU = "PETS-003", CategoryId = 9, Price = 14.99m, Quantity = random.Next(5, 50), LowStockThreshold = 15 },
            new Product { ProductId = 44, Name = "Pet Toys Set", Description = "Assorted pet toys", SKU = "PETS-004", CategoryId = 9, Price = 19.99m, Quantity = random.Next(5, 50), LowStockThreshold = 12 },
            new Product { ProductId = 45, Name = "Pet Carrier", Description = "Portable pet carrier", SKU = "PETS-005", CategoryId = 9, Price = 49.99m, Quantity = random.Next(5, 50), LowStockThreshold = 8 }
        });

        // Automotive
        products.AddRange(new[]
        {
            new Product { ProductId = 46, Name = "Car Phone Mount", Description = "Universal car phone mount", SKU = "AUTO-001", CategoryId = 10, Price = 19.99m, Quantity = random.Next(5, 50), LowStockThreshold = 12 },
            new Product { ProductId = 47, Name = "Car Vacuum Cleaner", Description = "Portable car vacuum", SKU = "AUTO-002", CategoryId = 10, Price = 39.99m, Quantity = random.Next(5, 50), LowStockThreshold = 10 },
            new Product { ProductId = 48, Name = "Car Air Freshener", Description = "Long-lasting air freshener", SKU = "AUTO-003", CategoryId = 10, Price = 7.99m, Quantity = random.Next(5, 50), LowStockThreshold = 20 },
            new Product { ProductId = 49, Name = "Tire Pressure Gauge", Description = "Digital tire pressure gauge", SKU = "AUTO-004", CategoryId = 10, Price = 12.99m, Quantity = random.Next(5, 50), LowStockThreshold = 15 },
            new Product { ProductId = 50, Name = "Car Wash Kit", Description = "Complete car wash kit", SKU = "AUTO-005", CategoryId = 10, Price = 29.99m, Quantity = random.Next(5, 50), LowStockThreshold = 10 }
        });

        modelBuilder.Entity<Product>().HasData(products);

        // Set up relationships
        modelBuilder.Entity<InventoryTask>()
            .HasOne(it => it.Inventory)
            .WithMany(i => i.Tasks)
            .HasForeignKey(it => it.InventoryId);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId);

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => oi.OrderId);

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Product)
            .WithMany(p => p.OrderItems)
            .HasForeignKey(oi => oi.ProductId);

        // Removed the incorrect seeding line for ApplicationUserCategory here

        modelBuilder.Entity<ApplicationUserCategory>()
            .HasKey(uc => new { uc.UserId, uc.CategoryId });

        modelBuilder.Entity<ApplicationUserCategory>()
            .HasOne(uc => uc.User)
            .WithMany(u => u.UserCategories)
            .HasForeignKey(uc => uc.UserId);

        modelBuilder.Entity<ApplicationUserCategory>()
            .HasOne(uc => uc.Category)
            .WithMany(c => c.UserCategories)
            .HasForeignKey(uc => uc.CategoryId);
    } // Closing brace for OnModelCreating
} // Closing brace for ApplicationDbContext
