using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RetailCompare.API.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "PriceHistories",
                columns: new[] { "StoreName", "Timestamp", "IsOnSale", "Price" },
                values: new object[,]
                {
                    { "SuperStore", new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, 32.99m },
                    { "ValueMart", new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Utc), false, 35.50m }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "CurrentLowestPrice", "ImageUrl", "Name" },
                values: new object[,]
                {
                    { 1, "Groceries", 32.99m, "https://via.placeholder.com/150", "Full Cream Milk 2L" },
                    { 2, "Bakery", 16.50m, "https://via.placeholder.com/150", "White Bread 700g" },
                    { 3, "Pantry", 119.99m, "https://via.placeholder.com/150", "Instant Coffee 200g" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PriceHistories",
                keyColumns: new[] { "StoreName", "Timestamp" },
                keyValues: new object[] { "SuperStore", new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.DeleteData(
                table: "PriceHistories",
                keyColumns: new[] { "StoreName", "Timestamp" },
                keyValues: new object[] { "ValueMart", new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
