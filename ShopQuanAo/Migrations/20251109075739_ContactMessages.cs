using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopQuanAo.Migrations
{
    /// <inheritdoc />
    public partial class ContactMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContactMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Website = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Message = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsReplied = table.Column<bool>(type: "bit", nullable: false),
                    RepliedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RepliedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReplySubject = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ReplyBody = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactMessages", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-001",
                columns: new[] { "ConcurrencyStamp", "CreatedDate", "PasswordHash", "SecurityStamp" },
                values: new object[] { "583ef9c6-cc39-4196-ab6c-3ee1525fe0f0", new DateTime(2025, 11, 9, 14, 57, 38, 465, DateTimeKind.Local).AddTicks(9055), "AQAAAAIAAYagAAAAEAPX5Jwqw6p+mi7aGDiIyVqL/rjX7DHLLbkFMuli85Ucgnua5FLHbC8YV7jAEIc1vw==", "7b6278a1-b279-44d0-9ba9-b37c9fae50f7" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 11, 9, 14, 57, 38, 562, DateTimeKind.Local).AddTicks(972));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 11, 9, 14, 57, 38, 562, DateTimeKind.Local).AddTicks(979));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 11, 9, 14, 57, 38, 562, DateTimeKind.Local).AddTicks(983));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2025, 11, 9, 14, 57, 38, 562, DateTimeKind.Local).AddTicks(986));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2025, 11, 9, 14, 57, 38, 562, DateTimeKind.Local).AddTicks(989));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContactMessages");

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
        }
    }
}
