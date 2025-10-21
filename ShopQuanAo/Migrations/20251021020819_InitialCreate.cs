using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ShopQuanAo.Migrations
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
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OldPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    IsFeatured = table.Column<bool>(type: "bit", nullable: false),
                    IsOnSale = table.Column<bool>(type: "bit", nullable: false),
                    StockQuantity = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "CreatedDate", "Description", "ImageUrl", "IsAvailable", "IsFeatured", "IsOnSale", "Name", "OldPrice", "Price", "StockQuantity", "UpdatedDate" },
                values: new object[,]
                {
                    { 1, "men", new DateTime(2025, 10, 21, 9, 8, 18, 546, DateTimeKind.Local).AddTicks(9414), "Áo thun nam chất liệu cotton cao cấp, thoáng mát", "~/images/product_1.png", true, true, true, "Áo Thun Nam Basic", 250000m, 199000m, 100, null },
                    { 2, "women", new DateTime(2025, 10, 21, 9, 8, 18, 546, DateTimeKind.Local).AddTicks(9419), "Áo sơ mi nữ công sở sang trọng", "~/images/product_2.png", true, true, false, "Áo Sơ Mi Nữ", null, 350000m, 50, null },
                    { 3, "men", new DateTime(2025, 10, 21, 9, 8, 18, 546, DateTimeKind.Local).AddTicks(9423), "Quần jeans nam form slim fit", "~/images/product_3.png", true, false, false, "Quần Jeans Nam", null, 450000m, 75, null },
                    { 4, "women", new DateTime(2025, 10, 21, 9, 8, 18, 546, DateTimeKind.Local).AddTicks(9427), "Váy đầm nữ thời trang cao cấp", "~/images/product_4.png", true, true, true, "Váy Đầm Nữ", 650000m, 550000m, 30, null },
                    { 5, "men", new DateTime(2025, 10, 21, 9, 8, 18, 546, DateTimeKind.Local).AddTicks(9430), "Áo khoác nam phong cách Hàn Quốc", "~/images/product_5.png", true, false, false, "Áo Khoác Nam", null, 650000m, 45, null }
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
