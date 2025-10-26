using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopQuanAo.Migrations
{
    /// <inheritdoc />
    public partial class AddSlides : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Slides",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Slides", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-001",
                columns: new[] { "ConcurrencyStamp", "CreatedDate", "PasswordHash", "SecurityStamp" },
                values: new object[] { "2e751fa7-0209-4cad-951b-3a310b63c2cc", new DateTime(2025, 10, 26, 19, 54, 53, 251, DateTimeKind.Local).AddTicks(4552), "AQAAAAIAAYagAAAAENQDMSCdxEPttvFEVQc+6awwa6aiynaNzlYTSrdcU/syeYZhLpYCeVD5FsHWsid6PA==", "5c8da6f1-f808-4ab0-9358-6a8f0dd70496" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 26, 19, 54, 53, 341, DateTimeKind.Local).AddTicks(6586));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 26, 19, 54, 53, 341, DateTimeKind.Local).AddTicks(6592));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 26, 19, 54, 53, 341, DateTimeKind.Local).AddTicks(6596));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 26, 19, 54, 53, 341, DateTimeKind.Local).AddTicks(6599));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 26, 19, 54, 53, 341, DateTimeKind.Local).AddTicks(6601));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Slides");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-001",
                columns: new[] { "ConcurrencyStamp", "CreatedDate", "PasswordHash", "SecurityStamp" },
                values: new object[] { "443c414f-fbc1-4097-bfc0-8c7c8a05f2ff", new DateTime(2025, 10, 22, 11, 48, 39, 972, DateTimeKind.Local).AddTicks(2032), "AQAAAAIAAYagAAAAEDjZsQwEL1ZKQLzJk2jhku2t51K6u3PQb3fTK+a94Fy+eA/377U4SKFMTUTz7s/6Cg==", "91fee2a7-6b8a-44a7-95e1-cb2624103631" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 22, 11, 48, 40, 75, DateTimeKind.Local).AddTicks(4599));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 22, 11, 48, 40, 75, DateTimeKind.Local).AddTicks(4604));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 22, 11, 48, 40, 75, DateTimeKind.Local).AddTicks(4608));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 22, 11, 48, 40, 75, DateTimeKind.Local).AddTicks(4612));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 22, 11, 48, 40, 75, DateTimeKind.Local).AddTicks(4614));
        }
    }
}
