using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BaratoInventory.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "CreatedAt", "Name", "Price", "Quantity", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "Electronics", new DateTime(2025, 11, 18, 4, 52, 41, 658, DateTimeKind.Utc).AddTicks(992), "Laptop", 999.99m, 10, null },
                    { 2, "Electronics", new DateTime(2025, 11, 18, 4, 52, 41, 658, DateTimeKind.Utc).AddTicks(993), "Mouse", 25.50m, 50, null },
                    { 3, "Electronics", new DateTime(2025, 11, 18, 4, 52, 41, 658, DateTimeKind.Utc).AddTicks(994), "Keyboard", 75.00m, 30, null },
                    { 4, "Electronics", new DateTime(2025, 11, 18, 4, 52, 41, 658, DateTimeKind.Utc).AddTicks(996), "Monitor", 299.99m, 15, null },
                    { 5, "Furniture", new DateTime(2025, 11, 18, 4, 52, 41, 658, DateTimeKind.Utc).AddTicks(997), "Desk Chair", 199.99m, 20, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
