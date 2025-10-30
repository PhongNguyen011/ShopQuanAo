using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopQuanAo.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderUserRef : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-001",
                columns: new[] { "ConcurrencyStamp", "CreatedDate", "PasswordHash", "SecurityStamp" },
                values: new object[] { "72a42b93-a5c2-401f-b31c-e9b218652160", new DateTime(2025, 10, 30, 15, 6, 32, 0, DateTimeKind.Local).AddTicks(7717), "AQAAAAIAAYagAAAAEL5zrW1fALcJaIrZNWpim/NhL7vOcKaxFq3FRHT0/tDIN1RxwwTShBT44oy51vx4eA==", "d9262d66-0b64-4e2d-9581-12e0dfe55ecb" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 30, 15, 6, 32, 83, DateTimeKind.Local).AddTicks(6162));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 30, 15, 6, 32, 83, DateTimeKind.Local).AddTicks(6166));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 30, 15, 6, 32, 83, DateTimeKind.Local).AddTicks(6170));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 30, 15, 6, 32, 83, DateTimeKind.Local).AddTicks(6174));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 30, 15, 6, 32, 83, DateTimeKind.Local).AddTicks(6178));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Orders");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-001",
                columns: new[] { "ConcurrencyStamp", "CreatedDate", "PasswordHash", "SecurityStamp" },
                values: new object[] { "734e121b-c1e9-43c6-9931-e506c9063eb6", new DateTime(2025, 10, 30, 15, 4, 15, 2, DateTimeKind.Local).AddTicks(8145), "AQAAAAIAAYagAAAAEICrzfYdJQMrDKTdOJFJ9JWgWFe496WKsJ62s/TlemB2SvPpQ/zai2/Rpsyoy8wuGg==", "36f388b8-d284-4d9b-8c62-cb9303abf265" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 30, 15, 4, 15, 90, DateTimeKind.Local).AddTicks(4892));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 30, 15, 4, 15, 90, DateTimeKind.Local).AddTicks(4896));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 30, 15, 4, 15, 90, DateTimeKind.Local).AddTicks(4900));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 30, 15, 4, 15, 90, DateTimeKind.Local).AddTicks(4904));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 30, 15, 4, 15, 90, DateTimeKind.Local).AddTicks(4907));
        }
    }
}
