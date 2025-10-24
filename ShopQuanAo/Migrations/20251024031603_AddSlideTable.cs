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
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Slides", x => x.Id);
                });

            // ✅ Cập nhật dữ liệu mặc định nếu có seed user admin
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-001",
                columns: new[] { "ConcurrencyStamp", "CreatedDate", "PasswordHash", "SecurityStamp" },
                values: new object[]
                {
                    Guid.NewGuid().ToString(),
                    DateTime.Now,
                    "AQAAAAIAAYagAAAAEEhLtoOLva97uhO+6+T3knaVExL6PaPcfhDY9LomRShB6LTa/S21XdsKwZ68BWq40w==",
                    Guid.NewGuid().ToString()
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Slides");

            // ✅ Khôi phục lại dữ liệu gốc (nếu rollback)
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-001",
                columns: new[] { "ConcurrencyStamp", "CreatedDate", "PasswordHash", "SecurityStamp" },
                values: new object[]
                {
                    "00000000-0000-0000-0000-000000000000",
                    DateTime.Now,
                    "AQAAAAIAAYagAAAAEIoAnEE1BAx222gM/Of6YuxHDNxGUdqLCDzvIYLyafJN1pS+xRFD9AishgdcZVIPmQ==",
                    "00000000-0000-0000-0000-000000000000"
                });
        }
    }
}
