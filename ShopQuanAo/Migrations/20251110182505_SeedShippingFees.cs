using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ShopQuanAo.Migrations
{
    /// <inheritdoc />
    public partial class SeedShippingFees : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-001",
                columns: new[] { "ConcurrencyStamp", "CreatedDate", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3891cd90-0945-4d08-a014-3248994dd981", new DateTime(2025, 11, 11, 1, 25, 4, 352, DateTimeKind.Local).AddTicks(1169), "AQAAAAIAAYagAAAAEBM+cV005lbJT+f9AFRwDDSsEnEJ5w+gtIrIgFtt3Ylj+UaElZEGvJtwqPSdzD35Zw==", "f69a47c6-dc84-4046-89ae-8036cce849cf" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 11, 11, 1, 25, 4, 433, DateTimeKind.Local).AddTicks(4526));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 11, 11, 1, 25, 4, 433, DateTimeKind.Local).AddTicks(4531));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 11, 11, 1, 25, 4, 433, DateTimeKind.Local).AddTicks(4537));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2025, 11, 11, 1, 25, 4, 433, DateTimeKind.Local).AddTicks(4544));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2025, 11, 11, 1, 25, 4, 433, DateTimeKind.Local).AddTicks(4548));

            migrationBuilder.InsertData(
                table: "ShippingFees",
                columns: new[] { "Id", "CreatedAt", "Description", "Fee", "IsActive", "ProvinceName", "UpdatedAt" },
                values: new object[,]
                {
                    { 101, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nội thành & lân cận", 35000m, true, "Thành phố Hà Nội", null },
                    { 102, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nội thành & lân cận", 35000m, true, "Thành phố Hồ Chí Minh", null },
                    { 103, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nội thành & lân cận", 35000m, true, "Thành phố Đà Nẵng", null },
                    { 104, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nội thành & lân cận", 35000m, true, "Thành phố Hải Phòng", null },
                    { 105, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nội thành & lân cận", 35000m, true, "Thành phố Cần Thơ", null },
                    { 106, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tỉnh miền núi", 55000m, true, "Tỉnh Hà Giang", null },
                    { 107, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tỉnh miền núi", 55000m, true, "Tỉnh Cao Bằng", null },
                    { 108, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tỉnh miền núi", 55000m, true, "Tỉnh Bắc Kạn", null },
                    { 109, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tỉnh miền núi", 55000m, true, "Tỉnh Tuyên Quang", null },
                    { 110, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tỉnh miền núi", 55000m, true, "Tỉnh Lào Cai", null },
                    { 111, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tỉnh miền núi", 55000m, true, "Tỉnh Điện Biên", null },
                    { 112, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tỉnh miền núi", 55000m, true, "Tỉnh Lai Châu", null },
                    { 113, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tỉnh miền núi", 55000m, true, "Tỉnh Sơn La", null },
                    { 114, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tỉnh miền núi", 55000m, true, "Tỉnh Yên Bái", null },
                    { 115, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tỉnh miền núi", 55000m, true, "Tỉnh Hòa Bình", null },
                    { 116, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Vùng núi phía Bắc", 55000m, true, "Tỉnh Lạng Sơn", null },
                    { 117, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tây Nguyên", 55000m, true, "Tỉnh Kon Tum", null },
                    { 118, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tây Nguyên", 55000m, true, "Tỉnh Gia Lai", null },
                    { 119, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tây Nguyên", 55000m, true, "Tỉnh Đắk Lắk", null },
                    { 120, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tây Nguyên", 55000m, true, "Tỉnh Đắk Nông", null },
                    { 121, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tây Nguyên", 55000m, true, "Tỉnh Lâm Đồng", null },
                    { 122, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ven biển", 45000m, true, "Tỉnh Quảng Ninh", null },
                    { 123, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", 45000m, true, "Tỉnh Bắc Giang", null },
                    { 124, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", 45000m, true, "Tỉnh Phú Thọ", null },
                    { 125, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", 45000m, true, "Tỉnh Vĩnh Phúc", null },
                    { 126, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", 45000m, true, "Tỉnh Thái Nguyên", null },
                    { 127, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", 45000m, true, "Tỉnh Bắc Ninh", null },
                    { 128, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", 45000m, true, "Tỉnh Hải Dương", null },
                    { 129, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", 45000m, true, "Tỉnh Hưng Yên", null },
                    { 130, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", 45000m, true, "Tỉnh Thái Bình", null },
                    { 131, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", 45000m, true, "Tỉnh Hà Nam", null },
                    { 132, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", 45000m, true, "Tỉnh Nam Định", null },
                    { 133, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", 45000m, true, "Tỉnh Ninh Bình", null },
                    { 134, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", 45000m, true, "Tỉnh Thanh Hóa", null },
                    { 135, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", 45000m, true, "Tỉnh Nghệ An", null },
                    { 136, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", 45000m, true, "Tỉnh Hà Tĩnh", null },
                    { 137, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", 45000m, true, "Tỉnh Quảng Bình", null },
                    { 138, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", 45000m, true, "Tỉnh Quảng Trị", null },
                    { 139, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", 45000m, true, "Tỉnh Thừa Thiên Huế", null },
                    { 140, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", 45000m, true, "Tỉnh Quảng Nam", null },
                    { 141, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", 45000m, true, "Tỉnh Quảng Ngãi", null },
                    { 142, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", 45000m, true, "Tỉnh Bình Định", null },
                    { 143, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", 45000m, true, "Tỉnh Phú Yên", null },
                    { 144, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ven biển", 45000m, true, "Tỉnh Khánh Hòa", null },
                    { 145, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ven biển", 45000m, true, "Tỉnh Ninh Thuận", null },
                    { 146, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ven biển", 45000m, true, "Tỉnh Bình Thuận", null },
                    { 147, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", 45000m, true, "Tỉnh Bình Phước", null },
                    { 148, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", 45000m, true, "Tỉnh Tây Ninh", null },
                    { 149, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", 45000m, true, "Tỉnh Bình Dương", null },
                    { 150, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", 45000m, true, "Tỉnh Đồng Nai", null },
                    { 151, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ven biển", 45000m, true, "Tỉnh Bà Rịa - Vũng Tàu", null },
                    { 152, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", 45000m, true, "Tỉnh Long An", null },
                    { 153, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", 45000m, true, "Tỉnh Tiền Giang", null },
                    { 154, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", 45000m, true, "Tỉnh Bến Tre", null },
                    { 155, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", 45000m, true, "Tỉnh Trà Vinh", null },
                    { 156, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", 45000m, true, "Tỉnh Vĩnh Long", null },
                    { 157, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", 45000m, true, "Tỉnh Đồng Tháp", null },
                    { 158, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", 45000m, true, "Tỉnh An Giang", null },
                    { 159, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ven biển", 45000m, true, "Tỉnh Kiên Giang", null },
                    { 160, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", 45000m, true, "Tỉnh Hậu Giang", null },
                    { 161, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ven biển", 45000m, true, "Tỉnh Sóc Trăng", null },
                    { 162, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ven biển", 45000m, true, "Tỉnh Bạc Liêu", null },
                    { 163, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ven biển", 45000m, true, "Tỉnh Cà Mau", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 102);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 103);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 104);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 105);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 106);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 107);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 108);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 109);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 110);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 111);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 112);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 113);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 114);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 115);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 116);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 117);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 118);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 119);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 120);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 121);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 122);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 123);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 124);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 125);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 126);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 127);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 128);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 129);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 130);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 131);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 132);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 133);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 134);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 135);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 136);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 137);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 138);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 139);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 140);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 141);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 142);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 143);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 144);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 145);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 146);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 147);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 148);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 149);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 150);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 151);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 152);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 153);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 154);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 155);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 156);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 157);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 158);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 159);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 160);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 161);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 162);

            migrationBuilder.DeleteData(
                table: "ShippingFees",
                keyColumn: "Id",
                keyValue: 163);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-001",
                columns: new[] { "ConcurrencyStamp", "CreatedDate", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d927c89e-166d-4057-9a8a-1ac5b33a32cf", new DateTime(2025, 11, 10, 23, 50, 4, 552, DateTimeKind.Local).AddTicks(9752), "AQAAAAIAAYagAAAAEK70dShBF4ZKf19dREh+6xf/CRhcLEJbaF0y4pgs6CY4KOGx5pUFnPrZXOXscleXwA==", "9244b978-ec13-4b2d-994e-581abfae51d9" });

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
    }
}
