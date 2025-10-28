using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopQuanAo.Migrations
{
    /// <inheritdoc />
    public partial class Order : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-001",
                columns: new[] { "ConcurrencyStamp", "CreatedDate", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8e50ef61-823e-4985-b80c-d2f39543e8b3", new DateTime(2025, 10, 29, 3, 41, 32, 792, DateTimeKind.Local).AddTicks(1399), "AQAAAAIAAYagAAAAEDsG6msIX8vsheS+FCSrmeVdsQrVrdsMGdgUTKZHasn0qKSSMdkPBne7p3B0VRXY5A==", "3e8fa0de-dfea-4192-83aa-c7a70dc20941" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 29, 3, 41, 32, 879, DateTimeKind.Local).AddTicks(4145));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 29, 3, 41, 32, 879, DateTimeKind.Local).AddTicks(4151));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 29, 3, 41, 32, 879, DateTimeKind.Local).AddTicks(4155));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 29, 3, 41, 32, 879, DateTimeKind.Local).AddTicks(4159));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 29, 3, 41, 32, 879, DateTimeKind.Local).AddTicks(4162));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-001",
                columns: new[] { "ConcurrencyStamp", "CreatedDate", "PasswordHash", "SecurityStamp" },
                values: new object[] { "eb3cc694-c870-42e8-a83c-bf0ce3536a40", new DateTime(2025, 10, 29, 3, 41, 5, 245, DateTimeKind.Local).AddTicks(3526), "AQAAAAIAAYagAAAAEGP5025zJEbAcyCksEo9LMHIBlSKgzxufIKBIfRNdHsRsC+u7pcmmFTJwox8f+htrw==", "990b367f-b610-412e-a588-15cf7e324855" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 29, 3, 41, 5, 331, DateTimeKind.Local).AddTicks(3303));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 29, 3, 41, 5, 331, DateTimeKind.Local).AddTicks(3307));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 29, 3, 41, 5, 331, DateTimeKind.Local).AddTicks(3311));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 29, 3, 41, 5, 331, DateTimeKind.Local).AddTicks(3315));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 29, 3, 41, 5, 331, DateTimeKind.Local).AddTicks(3319));
        }
    }
}
