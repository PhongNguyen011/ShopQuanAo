using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopQuanAo.Migrations
{
    /// <inheritdoc />
    public partial class AddPosts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(220)", maxLength: 220, nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Thumbnail = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                });

            var postTime = new DateTime(2025, 10, 29, 12, 0, 0, DateTimeKind.Utc);

            migrationBuilder.InsertData(
                table: "Posts",
                columns: new[] { "Id", "Title", "Slug", "Content", "Thumbnail", "CreatedAt", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "Khai trương cửa hàng", "khai-truong-cua-hang", "<p>Chào mừng đến với PNT Shop!</p>", "post-1.jpg", postTime, null },
                    { 2, "BST Thu Đông 2025", "bst-thu-dong-2025", "<p>Ra mắt BST mới.</p>", "post-2.jpg", postTime, null },
                    { 3, "Mẹo phối đồ basic", "meo-phoi-do-basic", "<p>Gợi ý phối đồ hằng ngày.</p>", "post-3.jpg", postTime, null }
                }
            );


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

            migrationBuilder.CreateIndex(
                name: "IX_Posts_Slug",
                table: "Posts",
                column: "Slug");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-001",
                columns: new[] { "ConcurrencyStamp", "CreatedDate", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c909806e-8cc7-43d9-a3cc-d94d1638935a", new DateTime(2025, 10, 21, 10, 35, 9, 550, DateTimeKind.Local).AddTicks(4661), "AQAAAAIAAYagAAAAEBcRhCnvpXZGd/YVk1kJVsqnusWp6sgm3do9BjkkN2jtxPVYPLmlXBqlkXy22jaU1g==", "e4fec669-3fb6-4953-941b-634e0e65b83b" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 21, 10, 35, 9, 648, DateTimeKind.Local).AddTicks(8546));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 21, 10, 35, 9, 648, DateTimeKind.Local).AddTicks(8551));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 21, 10, 35, 9, 648, DateTimeKind.Local).AddTicks(8555));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 21, 10, 35, 9, 648, DateTimeKind.Local).AddTicks(8559));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 21, 10, 35, 9, 648, DateTimeKind.Local).AddTicks(8562));
        }
    }
}
