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
            // Cho phép cột ImageUrl có thể null (nếu chưa có hình)
            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Slides",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            // ✅ Thêm cột CreatedAt
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Slides",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            // Cập nhật lại dữ liệu mẫu (nếu có seed data)
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
            // Xóa cột CreatedAt khi rollback
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Slides");

            // Đặt lại ImageUrl là NOT NULL
            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Slides",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
