using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopQuanAo.Migrations
{
    /// <inheritdoc />
    public partial class AddSlideTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Slides",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Slides",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-001",
                columns: new[] { "ConcurrencyStamp", "CreatedDate", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d67424c0-8204-453d-a0b0-6c9d7e7e66bd", new DateTime(2025, 10, 24, 10, 16, 2, 43, DateTimeKind.Local).AddTicks(3004), "AQAAAAIAAYagAAAAEEhLtoOLva97uhO+6+T3knaVExL6PaPcfhDY9LomRShB6LTa/S21XdsKwZ68BWq40w==", "a2954a91-fcb2-4ec0-a5e3-3dbc146f8e65" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 24, 10, 16, 2, 183, DateTimeKind.Local).AddTicks(7333));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 24, 10, 16, 2, 183, DateTimeKind.Local).AddTicks(7339));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 24, 10, 16, 2, 183, DateTimeKind.Local).AddTicks(7343));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 24, 10, 16, 2, 183, DateTimeKind.Local).AddTicks(7346));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 24, 10, 16, 2, 183, DateTimeKind.Local).AddTicks(7350));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Slides");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Slides",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-001",
                columns: new[] { "ConcurrencyStamp", "CreatedDate", "PasswordHash", "SecurityStamp" },
                values: new object[] { "6e41059f-a1d6-47c3-8c43-f5e4557695b0", new DateTime(2025, 10, 24, 8, 44, 9, 336, DateTimeKind.Local).AddTicks(8023), "AQAAAAIAAYagAAAAEIoAnEE1BAx222gM/Of6YuxHDNxGUdqLCDzvIYLyafJN1pS+xRFD9AishgdcZVIPmQ==", "423ec4c5-0c45-4526-9c66-c316469e0ce6" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 24, 8, 44, 9, 473, DateTimeKind.Local).AddTicks(9212));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 24, 8, 44, 9, 473, DateTimeKind.Local).AddTicks(9217));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 24, 8, 44, 9, 473, DateTimeKind.Local).AddTicks(9220));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 24, 8, 44, 9, 473, DateTimeKind.Local).AddTicks(9224));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 24, 8, 44, 9, 473, DateTimeKind.Local).AddTicks(9227));
        }
    }
}
