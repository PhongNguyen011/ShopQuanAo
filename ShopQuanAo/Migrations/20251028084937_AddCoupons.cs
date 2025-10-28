using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopQuanAo.Migrations
{
    /// <inheritdoc />
    public partial class AddCoupons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Tạo bảng Coupons với đầy đủ cột đúng theo Model
            migrationBuilder.CreateTable(
                name: "Coupons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DiscountType = table.Column<int>(type: "int", nullable: false),          // 0=Percentage, 1=FixedAmount
                    DiscountValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MinOrderAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Scope = table.Column<int>(type: "int", nullable: false),                  // 0=All, 1=CategoryOnly
                    AllowedCategoriesCsv = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupons", x => x.Id);
                });

            // Unique index cho Code
            migrationBuilder.CreateIndex(
                name: "IX_Coupons_Code",
                table: "Coupons",
                column: "Code",
                unique: true);

            // ====== SEED dữ liệu mẫu (tuỳ chọn) ======
            // Thời gian cố định để migration ổn định qua các máy
            var seedStart = new DateTime(2025, 01, 01, 00, 00, 00, DateTimeKind.Utc);
            var seedEnd = new DateTime(2025, 12, 31, 23, 59, 59, DateTimeKind.Utc);

            migrationBuilder.InsertData(
                table: "Coupons",
                columns: new[]
                {
                    "Id","Code","DiscountType","DiscountValue","MinOrderAmount",
                    "StartDate","EndDate","IsActive","Scope","AllowedCategoriesCsv",
                    "CreatedAt","UpdatedAt"
                },
                values: new object[,]
                {
                    // SALE10: giảm 10% cho tất cả đơn, không yêu cầu tối thiểu
                    {
                        1, "SALE10", 0, 10.00m, null,
                        seedStart, seedEnd, true, 0, null,
                        seedStart, null
                    },
                    // WOMEN50K: giảm 50k cho đơn tối thiểu 200k, chỉ áp dụng danh mục women
                    {
                        2, "WOMEN50K", 1, 50000.00m, 200000.00m,
                        seedStart, seedEnd, true, 1, "women",
                        seedStart, null
                    }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Coupons");
        }
    }
}