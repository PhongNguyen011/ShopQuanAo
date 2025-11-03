using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopQuanAo.Migrations
{
    /// <inheritdoc />
    public partial class AddWishlistTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WishlistItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WishlistItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WishlistItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-001",
                columns: new[] { "ConcurrencyStamp", "CreatedDate", "PasswordHash", "SecurityStamp" },
                values: new object[] { "2d613273-4bbf-499b-85f4-5f6d5f9b3443", new DateTime(2025, 11, 3, 14, 34, 24, 296, DateTimeKind.Local).AddTicks(7704), "AQAAAAIAAYagAAAAEEpVLDsBBwNVW1AE9wQw3CFbTAksvfprb1YTxukqB0UediIKldMb8WWCjNmS0VkIbQ==", "a84b5955-53c6-4b0d-9fb8-656fc9768a47" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 11, 3, 14, 34, 24, 379, DateTimeKind.Local).AddTicks(8426));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 11, 3, 14, 34, 24, 379, DateTimeKind.Local).AddTicks(8430));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 11, 3, 14, 34, 24, 379, DateTimeKind.Local).AddTicks(8434));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2025, 11, 3, 14, 34, 24, 379, DateTimeKind.Local).AddTicks(8438));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2025, 11, 3, 14, 34, 24, 379, DateTimeKind.Local).AddTicks(8441));

            migrationBuilder.CreateIndex(
                name: "IX_WishlistItems_ApplicationUserId_ProductId",
                table: "WishlistItems",
                columns: new[] { "ApplicationUserId", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WishlistItems_ProductId",
                table: "WishlistItems",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WishlistItems");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-001",
                columns: new[] { "ConcurrencyStamp", "CreatedDate", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ea865fd9-8caa-41bb-94cf-773a0782144c", new DateTime(2025, 11, 1, 2, 40, 33, 307, DateTimeKind.Local).AddTicks(9781), "AQAAAAIAAYagAAAAEN84dBNPf/OpSX42uTkemheTt4ZCP7cs8ng7OxWcd9fJEr7WYtq0teBj0S2QmhCklg==", "ada7c183-cc64-40fc-90fc-5c0d2525b63e" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 11, 1, 2, 40, 33, 382, DateTimeKind.Local).AddTicks(5540));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 11, 1, 2, 40, 33, 382, DateTimeKind.Local).AddTicks(5543));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 11, 1, 2, 40, 33, 382, DateTimeKind.Local).AddTicks(5545));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2025, 11, 1, 2, 40, 33, 382, DateTimeKind.Local).AddTicks(5548));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2025, 11, 1, 2, 40, 33, 382, DateTimeKind.Local).AddTicks(5550));
        }
    }
}
