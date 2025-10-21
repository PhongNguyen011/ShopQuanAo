# Hướng Dẫn Sử Dụng Shop Quần Áo

## Tổng Quan Hệ Thống

Dự án đã được cấu hình đầy đủ với:
- ✅ Model Product để quản lý sản phẩm
- ✅ Entity Framework Core với SQL Server LocalDB
- ✅ Admin Area với CRUD đầy đủ (Thêm, Sửa, Xóa, Xem)
- ✅ Shop Controller để hiển thị sản phẩm cho khách hàng
- ✅ Views đã được cập nhật với dữ liệu động

## Bước 1: Cài Đặt Dependencies

Mở Terminal trong Visual Studio hoặc Command Prompt tại thư mục dự án và chạy:

```bash
dotnet restore
```

## Bước 2: Tạo Database

### 2.1. Tạo Migration
```bash
dotnet ef migrations add InitialCreate
```

### 2.2. Cập nhật Database
```bash
dotnet ef database update
```

**Lưu ý:** Nếu chưa cài đặt Entity Framework Tools, chạy lệnh sau:
```bash
dotnet tool install --global dotnet-ef
```

## Bước 3: Chạy Dự Án

```bash
dotnet run
```

Hoặc nhấn **F5** trong Visual Studio.

## Cấu Trúc Dự Án

### 📁 Models
- **Product.cs**: Model sản phẩm với đầy đủ thuộc tính

### 📁 Data
- **ApplicationDbContext.cs**: DbContext với 5 sản phẩm mẫu

### 📁 Areas/Admin/Controllers
- **ProductController.cs**: Quản lý CRUD sản phẩm
- **HomeController.cs**: Dashboard Admin

### 📁 Areas/Admin/Views
- **Product/Index.cshtml**: Danh sách sản phẩm
- **Product/Create.cshtml**: Thêm sản phẩm mới
- **Product/Edit.cshtml**: Sửa sản phẩm
- **Product/Delete.cshtml**: Xóa sản phẩm
- **Product/Details.cshtml**: Chi tiết sản phẩm
- **Shared/_AdminLayout.cshtml**: Layout cho Admin

### 📁 Controllers
- **ShopController.cs**: Hiển thị sản phẩm cho khách hàng
- **HomeController.cs**: Trang chủ website

### 📁 Views/Shop
- **Index.cshtml**: Danh sách sản phẩm (có lọc theo danh mục)
- **Detail.cshtml**: Chi tiết sản phẩm + sản phẩm liên quan

## Các URL Quan Trọng

### Phần User (Khách Hàng)
- **Trang Chủ**: `https://localhost:xxxx/`
- **Danh sách sản phẩm**: `https://localhost:xxxx/Shop`
- **Sản phẩm Nam**: `https://localhost:xxxx/Shop?category=men`
- **Sản phẩm Nữ**: `https://localhost:xxxx/Shop?category=women`
- **Phụ kiện**: `https://localhost:xxxx/Shop?category=accessories`
- **Chi tiết sản phẩm**: `https://localhost:xxxx/Shop/Detail/1`
- **Liên hệ**: `https://localhost:xxxx/Home/Contact`

### Phần Admin
- **Dashboard Admin**: `https://localhost:xxxx/Admin`
- **Quản lý sản phẩm**: `https://localhost:xxxx/Admin/Product`
- **Thêm sản phẩm**: `https://localhost:xxxx/Admin/Product/Create`
- **Sửa sản phẩm**: `https://localhost:xxxx/Admin/Product/Edit/1`
- **Xóa sản phẩm**: `https://localhost:xxxx/Admin/Product/Delete/1`
- **Chi tiết sản phẩm**: `https://localhost:xxxx/Admin/Product/Details/1`

## Chức Năng Đã Hoàn Thành

### Admin Panel
✅ Thêm sản phẩm mới (có upload hình ảnh)
✅ Sửa sản phẩm (có thể thay đổi hình ảnh)
✅ Xóa sản phẩm (có xác nhận)
✅ Xem chi tiết sản phẩm
✅ Danh sách sản phẩm với trạng thái (Có sẵn, Nổi bật, Sale)
✅ Thông báo thành công/lỗi sau mỗi thao tác

### Shop (User)
✅ Hiển thị tất cả sản phẩm có sẵn
✅ Lọc sản phẩm theo danh mục (Nam, Nữ, Phụ Kiện)
✅ Hiển thị giá cũ/mới nếu có sale
✅ Badge hiển thị trạng thái (New, Sale)
✅ Chi tiết sản phẩm với số lượng tồn kho
✅ Sản phẩm liên quan (cùng danh mục)

## Database Schema

### Bảng Products

| Cột | Kiểu | Mô Tả |
|-----|------|-------|
| Id | int | Primary Key, Auto Increment |
| Name | nvarchar(200) | Tên sản phẩm (bắt buộc) |
| Description | nvarchar(1000) | Mô tả sản phẩm |
| Price | decimal(18,2) | Giá hiện tại (bắt buộc) |
| OldPrice | decimal(18,2) | Giá cũ (tùy chọn) |
| ImageUrl | nvarchar(500) | Đường dẫn hình ảnh |
| Category | nvarchar(100) | Danh mục (men/women/accessories) |
| IsAvailable | bit | Có sẵn hay không |
| IsFeatured | bit | Sản phẩm nổi bật |
| IsOnSale | bit | Đang giảm giá |
| StockQuantity | int | Số lượng tồn kho |
| CreatedDate | datetime2 | Ngày tạo |
| UpdatedDate | datetime2 | Ngày cập nhật |

## Dữ Liệu Mẫu

Database sẽ được seed với 5 sản phẩm mẫu:
1. Áo Thun Nam Basic - 199,000đ (Sale từ 250,000đ)
2. Áo Sơ Mi Nữ - 350,000đ
3. Quần Jeans Nam - 450,000đ
4. Váy Đầm Nữ - 550,000đ (Sale từ 650,000đ)
5. Áo Khoác Nam - 650,000đ

## Lưu Ý Quan Trọng

### Connection String
Mặc định sử dụng SQL Server LocalDB. Nếu muốn đổi database, sửa trong `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ShopQuanAoDB;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

### Upload Hình Ảnh
- Hình ảnh được lưu trong thư mục `wwwroot/images/`
- Tên file được tự động tạo unique bằng GUID
- Chấp nhận định dạng: jpg, jpeg, png, gif

### Navigation Menu
Tất cả các trang đều đã được cập nhật với ASP.NET Core Tag Helpers:
- `asp-area`: Xác định Area (Admin hoặc rỗng)
- `asp-controller`: Xác định Controller
- `asp-action`: Xác định Action

## Các Lệnh Hữu Ích

### Xóa Database và Tạo Lại
```bash
dotnet ef database drop
dotnet ef database update
```

### Tạo Migration Mới
```bash
dotnet ef migrations add TenMigration
```

### Xem Danh Sách Migration
```bash
dotnet ef migrations list
```

### Build Dự Án
```bash
dotnet build
```

### Publish Dự Án
```bash
dotnet publish -c Release
```

## Troubleshooting

### Lỗi: "Unable to resolve service for type 'ApplicationDbContext'"
- Đảm bảo đã chạy `dotnet restore`
- Kiểm tra connection string trong `appsettings.json`

### Lỗi: "A network-related or instance-specific error"
- Đảm bảo SQL Server LocalDB đã được cài đặt
- Thử chạy: `sqllocaldb start mssqllocaldb`

### Lỗi: Migration không tạo được
- Cài đặt EF Tools: `dotnet tool install --global dotnet-ef`
- Kiểm tra đã có package `Microsoft.EntityFrameworkCore.Tools`

## Mở Rộng Trong Tương Lai

Các tính năng có thể thêm:
- 🔐 Authentication & Authorization
- 🛒 Giỏ hàng và Checkout
- 💳 Tích hợp thanh toán online
- 📧 Gửi email xác nhận đơn hàng
- 📊 Báo cáo và thống kê doanh thu
- 🔍 Tìm kiếm sản phẩm nâng cao
- ⭐ Đánh giá và review sản phẩm
- 📱 Responsive design optimization
- 🎨 Quản lý màu sắc và size sản phẩm

## Liên Hệ & Hỗ Trợ

Nếu gặp vấn đề, vui lòng:
1. Kiểm tra lại các bước trong hướng dẫn
2. Xem phần Troubleshooting
3. Kiểm tra console log để xem lỗi chi tiết

---
**Chúc bạn code vui vẻ! 🎉**

