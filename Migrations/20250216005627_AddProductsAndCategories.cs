using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace COMP2139_assign01.Migrations
{
    /// <inheritdoc />
    public partial class AddProductsAndCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Inventory",
                keyColumn: "InventoryId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Inventory",
                keyColumn: "InventoryId",
                keyValue: 2);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 2,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Books and educational materials", "Books" });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "Description", "Name" },
                values: new object[,]
                {
                    { 3, "Apparel and fashion items", "Clothing" },
                    { 4, "Home and kitchen appliances", "Home & Kitchen" },
                    { 5, "Sports equipment and outdoor gear", "Sports & Outdoors" },
                    { 6, "Toys, games, and entertainment items", "Toys & Games" },
                    { 7, "Beauty and personal care products", "Beauty & Personal Care" },
                    { 8, "Office and stationery items", "Office Supplies" },
                    { 9, "Pet food and accessories", "Pet Supplies" },
                    { 10, "Car accessories and maintenance items", "Automotive" }
                });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1,
                columns: new[] { "Description", "ImageUrl", "LowStockThreshold", "Name", "Price", "Quantity", "SKU" },
                values: new object[] { "High-quality wireless earbuds with noise cancellation", null, 10, "Wireless Earbuds", 79.99m, 49, "ELEC-001" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 2,
                columns: new[] { "CategoryId", "Description", "ImageUrl", "LowStockThreshold", "Name", "Price", "Quantity", "SKU" },
                values: new object[] { 1, "Feature-rich smartwatch with health tracking", null, 8, "Smart Watch", 199.99m, 45, "ELEC-002" });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "CategoryId", "Description", "ImageUrl", "LowStockThreshold", "Name", "Price", "Quantity", "SKU" },
                values: new object[,]
                {
                    { 3, 1, "Portable Bluetooth speaker with great sound", null, 12, "Bluetooth Speaker", 49.99m, 38, "ELEC-003" },
                    { 4, 1, "Fast-charging USB-C power adapter", null, 15, "USB-C Charger", 24.99m, 41, "ELEC-004" },
                    { 5, 1, "Ergonomic wireless mouse", null, 10, "Wireless Mouse", 29.99m, 38, "ELEC-005" },
                    { 6, 2, "Comprehensive programming guide", null, 7, "The Art of Programming", 39.99m, 7, "BOOK-001" },
                    { 7, 2, "Introduction to data science", null, 7, "Data Science Basics", 29.99m, 5, "BOOK-002" },
                    { 8, 2, "Modern web development techniques", null, 7, "Web Development Guide", 34.99m, 11, "BOOK-003" },
                    { 9, 2, "AI and ML fundamentals", null, 7, "AI & Machine Learning", 44.99m, 13, "BOOK-004" },
                    { 10, 2, "Introduction to cloud computing", null, 7, "Cloud Computing 101", 32.99m, 33, "BOOK-005" },
                    { 11, 3, "Comfortable cotton t-shirt", null, 15, "Cotton T-Shirt", 19.99m, 45, "CLTH-001" },
                    { 12, 3, "Classic denim jeans", null, 10, "Denim Jeans", 49.99m, 27, "CLTH-002" },
                    { 13, 3, "Warm hooded sweatshirt", null, 12, "Hooded Sweatshirt", 39.99m, 13, "CLTH-003" },
                    { 14, 3, "Comfortable running shoes", null, 8, "Running Shoes", 89.99m, 25, "CLTH-004" },
                    { 15, 3, "Warm winter jacket", null, 8, "Winter Jacket", 129.99m, 8, "CLTH-005" },
                    { 16, 4, "Programmable coffee maker", null, 8, "Coffee Maker", 79.99m, 43, "HOME-001" },
                    { 17, 4, "Multi-function toaster oven", null, 8, "Toaster Oven", 69.99m, 30, "HOME-002" },
                    { 18, 4, "High-speed blender", null, 10, "Blender", 49.99m, 16, "HOME-003" },
                    { 19, 4, "Complete cookware set", null, 6, "Cookware Set", 149.99m, 5, "HOME-004" },
                    { 20, 4, "Professional kitchen knife set", null, 7, "Kitchen Knife Set", 89.99m, 47, "HOME-005" },
                    { 21, 5, "Non-slip yoga mat", null, 12, "Yoga Mat", 24.99m, 12, "SPRT-001" },
                    { 22, 5, "Adjustable dumbbells set", null, 8, "Dumbbells Set", 79.99m, 6, "SPRT-002" },
                    { 23, 5, "Professional tennis racket", null, 8, "Tennis Racket", 59.99m, 40, "SPRT-003" },
                    { 24, 5, "Indoor/outdoor basketball", null, 10, "Basketball", 29.99m, 5, "SPRT-004" },
                    { 25, 5, "4-person camping tent", null, 6, "Camping Tent", 199.99m, 38, "SPRT-005" },
                    { 26, 6, "Classic board games collection", null, 10, "Board Game Set", 34.99m, 33, "TOYS-001" },
                    { 27, 6, "High-speed RC car", null, 8, "Remote Control Car", 44.99m, 35, "TOYS-002" },
                    { 28, 6, "Creative building blocks set", null, 12, "Building Blocks", 29.99m, 45, "TOYS-003" },
                    { 29, 6, "1000-piece puzzle set", null, 15, "Puzzle Set", 19.99m, 28, "TOYS-004" },
                    { 30, 6, "Complete art supply kit", null, 10, "Art Supply Kit", 24.99m, 11, "TOYS-005" },
                    { 31, 7, "Professional hair dryer", null, 10, "Hair Dryer", 49.99m, 31, "BEAU-001" },
                    { 32, 7, "Sonic electric toothbrush", null, 12, "Electric Toothbrush", 39.99m, 43, "BEAU-002" },
                    { 33, 7, "Hydrating face moisturizer", null, 15, "Face Moisturizer", 24.99m, 39, "BEAU-003" },
                    { 34, 7, "Premium shampoo and conditioner", null, 15, "Shampoo Set", 29.99m, 8, "BEAU-004" },
                    { 35, 7, "Professional makeup kit", null, 8, "Makeup Kit", 59.99m, 31, "BEAU-005" },
                    { 36, 8, "Multi-compartment desk organizer", null, 12, "Desk Organizer", 19.99m, 9, "OFFC-001" },
                    { 37, 8, "Premium notebook set", null, 15, "Notebook Set", 14.99m, 46, "OFFC-002" },
                    { 38, 8, "High-quality printer paper", null, 20, "Printer Paper", 9.99m, 14, "OFFC-003" },
                    { 39, 8, "Luxury pen set", null, 20, "Pen Set", 7.99m, 23, "OFFC-004" },
                    { 40, 8, "Heavy-duty stapler", null, 15, "Stapler", 8.99m, 40, "OFFC-005" },
                    { 41, 9, "Comfortable dog bed", null, 8, "Dog Bed", 39.99m, 46, "PETS-001" },
                    { 42, 9, "Self-cleaning litter box", null, 10, "Cat Litter Box", 29.99m, 44, "PETS-002" },
                    { 43, 9, "Stainless steel food bowl", null, 15, "Pet Food Bowl", 14.99m, 27, "PETS-003" },
                    { 44, 9, "Assorted pet toys", null, 12, "Pet Toys Set", 19.99m, 13, "PETS-004" },
                    { 45, 9, "Portable pet carrier", null, 8, "Pet Carrier", 49.99m, 39, "PETS-005" },
                    { 46, 10, "Universal car phone mount", null, 12, "Car Phone Mount", 19.99m, 10, "AUTO-001" },
                    { 47, 10, "Portable car vacuum", null, 10, "Car Vacuum Cleaner", 39.99m, 13, "AUTO-002" },
                    { 48, 10, "Long-lasting air freshener", null, 20, "Car Air Freshener", 7.99m, 22, "AUTO-003" },
                    { 49, 10, "Digital tire pressure gauge", null, 15, "Tire Pressure Gauge", 12.99m, 12, "AUTO-004" },
                    { 50, 10, "Complete car wash kit", null, 10, "Car Wash Kit", 29.99m, 26, "AUTO-005" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 10);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 2,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Household items and furniture", "Household" });

            migrationBuilder.InsertData(
                table: "Inventory",
                columns: new[] { "InventoryId", "Name", "Price", "Quantity", "TotalStock", "category" },
                values: new object[,]
                {
                    { 1, "Carpet", 50m, 5, 0, "Household" },
                    { 2, "Laptop", 500m, 7, 0, "Electronics" }
                });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1,
                columns: new[] { "Description", "ImageUrl", "LowStockThreshold", "Name", "Price", "Quantity", "SKU" },
                values: new object[] { "High-performance laptop with latest specifications", "/images/laptop.jpg", 5, "Laptop", 500m, 7, "ELEC-LAP-001" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 2,
                columns: new[] { "CategoryId", "Description", "ImageUrl", "LowStockThreshold", "Name", "Price", "Quantity", "SKU" },
                values: new object[] { 2, "Luxurious carpet for your living room", "/images/carpet.jpg", 3, "Carpet", 50m, 5, "HOME-CAR-001" });
        }
    }
}
