using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopQuanAo.Migrations
{
    /// <inheritdoc />
    public partial class AddShippingFeesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 👉 Tạo bảng ShippingFees
            migrationBuilder.CreateTable(
                name: "ShippingFees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProvinceName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Fee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingFees", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShippingFees_ProvinceName",
                table: "ShippingFees",
                column: "ProvinceName");

            // 👉 Update dữ liệu seed sẵn
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-001",
                columns: new[] { "ConcurrencyStamp", "CreatedDate", "PasswordHash", "SecurityStamp" },
                values: new object[] {
                    "d927c89e-166d-4057-9a8a-1ac5b33a32cf",
                    new DateTime(2025, 11, 10, 23, 50, 4, 552, DateTimeKind.Local).AddTicks(9752),
                    "AQAAAAIAAYagAAAAEK70dShBF4ZKf19dREh+6xf/CRhcLEJbaF0y4pgs6CY4KOGx5pUFnPrZXOXscleXwA==",
                    "9244b978-ec13-4b2d-994e-581abfae51d9"
                });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 11, 10, 23, 50, 4, 671, DateTimeKind.Local).AddTicks(7844));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 11, 10, 23, 50, 4, 671, DateTimeKind.Local).AddTicks(7850));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 11, 10, 23, 50, 4, 671, DateTimeKind.Local).AddTicks(7855));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2025, 11, 10, 23, 50, 4, 671, DateTimeKind.Local).AddTicks(7861));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2025, 11, 10, 23, 50, 4, 671, DateTimeKind.Local).AddTicks(7865));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // 👉 Xóa bảng ShippingFees nếu rollback
            migrationBuilder.DropTable(
                name: "ShippingFees");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-001",
                columns: new[] { "ConcurrencyStamp", "CreatedDate", "PasswordHash", "SecurityStamp" },
                values: new object[] {
                    "f0f6d7ef-73f7-4f8c-ab51-734132030e06",
                    new DateTime(2025, 11, 10, 21, 10, 33, 947, DateTimeKind.Local).AddTicks(9185),
                    "AQAAAAIAAYagAAAAEIGUsYSno+bx6VzgdATeTsNwIHgLJKIOeET5cOiTsjOvXbCiBd+l0FntscXvCVbnhg==",
                    "6f6bfb6c-1e61-4e6f-adf7-23cb712c88b8"
                });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 11, 10, 21, 10, 34, 30, DateTimeKind.Local).AddTicks(1255));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 11, 10, 21, 10, 34, 30, DateTimeKind.Local).AddTicks(1263));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 11, 10, 21, 10, 34, 30, DateTimeKind.Local).AddTicks(1269));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2025, 11, 10, 21, 10, 34, 30, DateTimeKind.Local).AddTicks(1274));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2025, 11, 10, 21, 10, 34, 30, DateTimeKind.Local).AddTicks(1279));
        }
    }
}
