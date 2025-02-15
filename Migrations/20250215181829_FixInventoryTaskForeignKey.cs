using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace COMP2139_assign01.Migrations
{
    /// <inheritdoc />
    public partial class FixInventoryTaskForeignKey : Migration
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Inventory",
                columns: new[] { "InventoryId", "Category", "Name", "Price", "Quantity", "TotalStock", "category" },
                values: new object[,]
                {
                    { 1, "", null, "Carpet", 50m, 5, 0, "Household" },
                    { 2, "", null, "Laptop", 500m, 7, 0, "Electronics" }
                });
        }
    }
}
