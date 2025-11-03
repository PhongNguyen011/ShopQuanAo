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

            migrationBuilder.InsertData(
                table: "Posts",
                columns: new[] { "Id", "Title", "Slug", "Content", "Thumbnail", "CreatedAt", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "Khai trương cửa hàng",        "khai-truong-cua-hang",         "<p>Khai trương là sự kiện quan trọng đánh dấu sự khởi đầu mới của một doanh nghiệp. Việc viết content khai trương thu hút sẽ giúp thu hút sự chú ý của khách hàng, tạo tiếng vang cho thương hiệu và thúc đẩy doanh số bán hàng.</p>",                                     "post-1.jpg", new DateTime(2015, 1, 10, 9, 0, 0, DateTimeKind.Utc), null },
                    { 2, "BST Xuân Hè 2016",            "bst-xuan-he-2016",             "<p>Trong khi Valentino thể hiện phong cách Italy theo chiều hướng sang trọng, cổ điển, có chút quý tộc và xa cách thì Dolce & Gabbana lại gần gũi, tươi vui, gợi nhiều cảm xúc bằng chất cổ tích và truyền thống dân gian.\r\n\r\n\r\n</p>",                               "post-2.jpg", new DateTime(2016, 2, 15, 10, 30, 0, DateTimeKind.Utc), null },
                    { 3, "Mẹo phối đồ 2017",            "meo-phoi-do-2017",             "<p>Con gái lúc nào cũng phải xinh đẹp, và càng dễ thương bao nhiêu thì sẽ càng được yêu mến hơn bấy nhiêu</p>",                                                                                                                                                            "post-3.jpg", new DateTime(2017, 3, 12, 14, 45, 0, DateTimeKind.Utc), null },
                    { 4, "Ưu đãi Hè 2018",              "uu-dai-he-2018",               "<p>Đôi khi chúng ta chăm chăm tập trung vào sản phẩm, vào dịch vụ, vào các công dụng, các thông tin và nội dung bán hàng quá nên quên mất rằng những thứ mang tính giải trí sẽ làm tăng thiện cảm, tính gần gũi cho đôi bên.</p>",                                         "post-4.jpg", new DateTime(2018, 4, 20, 8, 0, 0, DateTimeKind.Utc), null },
                    { 5, "Cách bảo quản quần áo 2019",  "cach-bao-quan-quan-ao-2019",   "<p>Nếu đồ len mau xuống cấp, hẳn bạn đã chăm sóc chúng sai cách. Thay vì phải đến tiệm giặt ủi, thaham khảo 6 bí quyết bảo quản đồ len sau đây để tiết kiệm thời gian và chi phí.</p>",                                                                                    "post-5.jpg", new DateTime(2019, 5, 18, 11, 20, 0, DateTimeKind.Utc), null },
                    { 6, "Top áo khoác hot 2020",       "top-ao-khoac-hot-2020",        "<p>Áo khoác túi hộp hay còn gọi là áo khoác shacket, là sự kết hợp giữa áo sơ mi (shirt) và áo khoác (jacket). Với phom dáng rộng, chất vải cứng cáp cùng những chiếc túi vuông tiện lợi, shacket là kiểu áo khoác phù hợp với cả bốn mùa trong năm. </p>",                "post-6.jpg", new DateTime(2020, 6, 25, 13, 0, 0, DateTimeKind.Utc), null },
                    { 7, "Mix đồ đi học 2021",          "mix-do-di-hoc-2021",           "<p>Áo sơ mi trắng: Trong số các mẫu áo sơ mi, áo sơ mi trắng là lựa chọn phổ biến nhất bởi sự đơn giản, thanh lịch và dễ dàng kết hợp với nhiều trang phục khác.</p>",                                                                                                     "post-7.jpg", new DateTime(2021, 7, 5, 9, 45, 0, DateTimeKind.Utc), null },
                    { 8, "BST Thu Đông 2022",           "bst-thu-dong-2022",            "<p>Thời trang thể thao luôn là xu hướng trẻ mãi với thời gian bởi sự phóng khoáng, năng động và khỏe khoắn mà nó mang lại.</p>",                                                                                                                                           "post-8.jpg", new DateTime(2022, 8, 10, 16, 15, 0, DateTimeKind.Utc), null },
                    { 9, "Phụ kiện hot 2023",           "phu-kien-hot-2023",            "<p>Phụ kiện hot 2023 cho nữ bao gồm túi xách oversized, trang sức chunky, và khăn choàng lụa. Các món đồ này được kết hợp với trang phục như áo sơ mi crop-top, chân váy midi và quần ống rộng để tạo nên phong cách nữ tính, cá tính hoặc năng động, thoải mái. </p>",    "post-9.jpg", new DateTime(2023, 9, 8, 18, 30, 0, DateTimeKind.Utc), null },
                    { 10, "Ra mắt BST Thu Đông 2025",   "ra-mat-bst-thu-dong-2025",     "<p>Đã đến lúc phái mạnh hiện đại nâng tầm phong cách để khẳng định dấu ấn riêng, thể hiện khí chất trong mọi hành trình.</p>",                                                                                                                                             "post-10.jpg", new DateTime(2025, 10, 3, 10, 0, 0, DateTimeKind.Utc), null }
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
