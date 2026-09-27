using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RetailCompare.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialSchemaUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Watchlists");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PriceHistories",
                table: "PriceHistories");

            migrationBuilder.DeleteData(
                table: "PriceHistories",
                keyColumns: new[] { "StoreName", "Timestamp" },
                keyValues: new object[] { "SuperStore", new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.DeleteData(
                table: "PriceHistories",
                keyColumns: new[] { "StoreName", "Timestamp" },
                keyValues: new object[] { "ValueMart", new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.RenameColumn(
                name: "CurrentLowestPrice",
                table: "Products",
                newName: "StoreName");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Products",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Products",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Products",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "PriceHistories",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "PriceHistories",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PriceHistories",
                table: "PriceHistories",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    FullName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WatchlistItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    TargetPrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WatchlistItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WatchlistItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WatchlistItems_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "PriceHistories",
                columns: new[] { "Id", "IsOnSale", "Price", "ProductId", "StoreName", "Timestamp" },
                values: new object[,]
                {
                    { 1, true, 32.99m, 1, "SuperStore", new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, false, 35.50m, 1, "ValueMart", new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Price", "StoreName" },
                values: new object[] { "Fresh full cream milk", 32.99m, "SuperStore" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "Price", "StoreName" },
                values: new object[] { "Sliced white bread", 16.50m, "ValueMart" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "Price", "StoreName" },
                values: new object[] { "Rich roasted instant coffee", 119.99m, "SuperStore" });

            migrationBuilder.CreateIndex(
                name: "IX_PriceHistories_ProductId",
                table: "PriceHistories",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_WatchlistItems_ProductId",
                table: "WatchlistItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_WatchlistItems_UserId_ProductId",
                table: "WatchlistItems",
                columns: new[] { "UserId", "ProductId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PriceHistories_Products_ProductId",
                table: "PriceHistories",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PriceHistories_Products_ProductId",
                table: "PriceHistories");

            migrationBuilder.DropTable(
                name: "WatchlistItems");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PriceHistories",
                table: "PriceHistories");

            migrationBuilder.DropIndex(
                name: "IX_PriceHistories_ProductId",
                table: "PriceHistories");

            migrationBuilder.DeleteData(
                table: "PriceHistories",
                keyColumn: "Id",
                keyColumnType: "INTEGER",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "PriceHistories",
                keyColumn: "Id",
                keyColumnType: "INTEGER",
                keyValue: 2);

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "PriceHistories");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "PriceHistories");

            migrationBuilder.RenameColumn(
                name: "StoreName",
                table: "Products",
                newName: "CurrentLowestPrice");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Products",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PriceHistories",
                table: "PriceHistories",
                columns: new[] { "StoreName", "Timestamp" });

            migrationBuilder.CreateTable(
                name: "Watchlists",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    TargetPrice = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Watchlists", x => new { x.UserId, x.ProductId });
                });

            migrationBuilder.InsertData(
                table: "PriceHistories",
                columns: new[] { "StoreName", "Timestamp", "IsOnSale", "Price" },
                values: new object[,]
                {
                    { "SuperStore", new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, 32.99m },
                    { "ValueMart", new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Utc), false, 35.50m }
                });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "CurrentLowestPrice",
                value: 32.99m);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "CurrentLowestPrice",
                value: 16.50m);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "CurrentLowestPrice",
                value: 119.99m);
        }
    }
}
