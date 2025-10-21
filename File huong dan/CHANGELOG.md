# Changelog - Shop Quần Áo

## [Version 2.0] - 2025-10-21

### 🎉 Tính Năng Mới

#### 1. Hệ Thống Email Thực Sự
- ✅ **Gửi email xác thực qua Gmail SMTP**
  - Không còn hiển thị link qua CMD
  - Email được gửi trực tiếp đến hộp thư đăng ký
  - Hỗ trợ Gmail App Password
  - Template email đẹp với HTML/CSS
  - Hướng dẫn chi tiết trong `HUONG_DAN_EMAIL.md`

- ✅ **Các loại email được gửi:**
  - Email xác thực tài khoản khi đăng ký
  - Email đặt lại mật khẩu
  - Email chào mừng (template đẹp)

#### 2. Quản Lý Người Dùng Hoàn Chỉnh (Admin)
- ✅ **Trang User Management mới** (`/Admin/User`)
  - Hiển thị danh sách tất cả người dùng
  - Thông tin đầy đủ: Họ tên, Email, SĐT, Ngày tạo
  - Trạng thái tài khoản: Đã xác thực email, Hoạt động/Khóa
  - Hiển thị vai trò của từng người dùng
  - Giao diện bảng hiện đại với Argon Dashboard

- ✅ **Chỉnh sửa người dùng** (`/Admin/User/Edit/{id}`)
  - Cập nhật thông tin cá nhân (Email, Họ tên, SĐT)
  - **Phân quyền trực tiếp** - Gán/bỏ vai trò cho người dùng
  - Hỗ trợ nhiều vai trò cho 1 người dùng
  - Kích hoạt/Khóa tài khoản
  - Xác thực email thủ công (cho admin)
  - Validation đầy đủ

- ✅ **Xóa người dùng** (`/Admin/User/Delete/{id}`)
  - Hiển thị đầy đủ thông tin trước khi xóa
  - Xác nhận trước khi xóa
  - Không cho admin tự xóa chính mình
  - Không cho admin tự khóa chính mình

- ✅ **Tính năng bổ sung:**
  - Khóa/Mở khóa tài khoản nhanh (Toggle Active)
  - Dropdown menu actions cho mỗi người dùng
  - Badges màu sắc hiển thị trạng thái
  - Thông báo thành công/lỗi
  - Avatar với chữ cái đầu tên

#### 3. Cải Tiến Quản Lý Vai Trò (Admin)
- ✅ **Vai trò chỉ để tạo/xóa** (`/Admin/Role`)
  - Loại bỏ chức năng "Manage Users" khỏi Role
  - Phân quyền người dùng được chuyển sang User Management
  - Giao diện đơn giản, tập trung vào việc quản lý vai trò
  - Bảo vệ vai trò hệ thống (Admin, User không thể xóa)
  - Badge hiển thị "Vai trò hệ thống" cho Admin/User

- ✅ **Tách biệt chức năng:**
  - **Role Controller**: Tạo và xóa vai trò
  - **User Controller**: Gán vai trò cho người dùng
  - Logic rõ ràng, dễ bảo trì

### 🔧 Cải Tiến Kỹ Thuật

#### EmailService
- Loại bỏ demo mode (console log)
- Hỗ trợ gửi email thực qua SMTP
- Error handling tốt hơn
- Logging chi tiết với icon ✓/✗
- Hướng dẫn cấu hình rõ ràng khi chưa setup

#### ViewModels
- Thêm `UserManagementViewModel` - Hiển thị danh sách người dùng
- Thêm `EditUserViewModel` - Chỉnh sửa người dùng và phân quyền
- Thêm `RoleSelectionViewModel` - Checkbox chọn vai trò

#### Admin Layout
- Thêm menu "Người Dùng" vào navigation
- Icon phân biệt rõ ràng (Dashboard, Sản phẩm, Người dùng, Vai trò)
- Breadcrumb hiển thị đường dẫn

### 📝 Tài Liệu

- ✅ **HUONG_DAN_EMAIL.md** - Hướng dẫn chi tiết cấu hình email
  - Cách tạo App Password từ Google
  - Cấu hình appsettings.json
  - Troubleshooting các lỗi thường gặp
  - Hướng dẫn sử dụng User Secrets
  - Best practices cho production

- ✅ **HUONG_DAN_SU_DUNG.md** - Cập nhật đầy đủ
  - Thêm phần Authentication & Authorization
  - Thêm phần Quản lý người dùng
  - Thêm phần Quản lý vai trò
  - Cập nhật các URL quan trọng
  - Thêm tài khoản mặc định
  - Thêm troubleshooting email

- ✅ **CHANGELOG.md** - Lịch sử thay đổi (file này)

### 🎨 Giao Diện

#### User Management
- Table responsive với Argon Dashboard theme
- Avatar tròn với chữ cái đầu tên
- Badges màu sắc cho trạng thái:
  - 🔴 Admin role (bg-gradient-danger)
  - 🔵 User role (bg-gradient-info)
  - ✅ Email confirmed (bg-gradient-success)
  - ⚠️ Email not confirmed (bg-gradient-warning)
  - ✅ Active account (bg-gradient-success)
  - 🔒 Locked account (bg-gradient-danger)
- Dropdown menu actions hiện đại
- Form validation với Bootstrap

#### Role Management
- Đơn giản hóa, chỉ hiển thị tên và ID vai trò
- Badge "Vai trò hệ thống" cho Admin/User
- Nút xóa chỉ hiện với vai trò có thể xóa

### 🔒 Bảo Mật

- ✅ Không cho admin tự xóa chính mình
- ✅ Không cho admin tự khóa chính mình  
- ✅ Không cho xóa vai trò hệ thống (Admin, User)
- ✅ Email validation đầy đủ
- ✅ Anti-forgery token cho tất cả POST requests
- ✅ Authorization check cho tất cả admin actions
- ✅ Hướng dẫn sử dụng User Secrets (không commit email/password)

### 🐛 Bug Fixes

- ✅ Sửa lỗi email chỉ hiển thị trong console
- ✅ Sửa lỗi không có cách nào phân quyền user thuận tiện
- ✅ Sửa lỗi ManageUsers trong Role gây rối
- ✅ Cải thiện error messages
- ✅ Validation messages rõ ràng hơn

### 📦 Files Thêm Mới

#### Controllers
- `ShopQuanAo/Areas/Admin/Controllers/UserController.cs`

#### Views
- `ShopQuanAo/Areas/Admin/Views/User/Index.cshtml`
- `ShopQuanAo/Areas/Admin/Views/User/Edit.cshtml`
- `ShopQuanAo/Areas/Admin/Views/User/Delete.cshtml`

#### ViewModels
- `ShopQuanAo/Models/ViewModels/UserManagementViewModel.cs`

#### Documentation
- `HUONG_DAN_EMAIL.md`
- `CHANGELOG.md`

### 📦 Files Đã Xóa
- `ShopQuanAo/Areas/Admin/Views/Role/ManageUsers.cshtml` (Không còn dùng)

### 📦 Files Đã Cập Nhật

#### Services
- `ShopQuanAo/Services/EmailService.cs` - Gửi email thực, không demo mode

#### Controllers
- `ShopQuanAo/Controllers/AccountController.cs` - Bỏ hiển thị link confirm trong TempData
- `ShopQuanAo/Areas/Admin/Controllers/RoleController.cs` - Bỏ ManageUsers

#### Views
- `ShopQuanAo/Areas/Admin/Views/Shared/_AdminLayout.cshtml` - Thêm menu User
- `ShopQuanAo/Areas/Admin/Views/Role/Index.cshtml` - Bỏ nút ManageUsers

#### Documentation
- `HUONG_DAN_SU_DUNG.md` - Cập nhật toàn bộ

---

## [Version 1.0] - 2025-10-21 (Trước đó)

### Tính Năng Ban Đầu
- ✅ ASP.NET Core Identity với đăng ký/đăng nhập
- ✅ Quản lý sản phẩm (CRUD)
- ✅ Admin Area với Argon Dashboard
- ✅ Shop frontend cho khách hàng
- ✅ Role-based authorization
- ✅ Email confirmation (qua console log)
- ✅ Password reset
- ✅ Product management với upload ảnh
- ✅ Category filtering
- ✅ SQL Server LocalDB

---

## Migration từ Version 1.0 lên 2.0

### Không cần migration database
Database schema không thay đổi, chỉ thêm chức năng.

### Cần làm gì?
1. Pull code mới nhất
2. Cấu hình email trong `appsettings.json` (xem `HUONG_DAN_EMAIL.md`)
3. Chạy lại ứng dụng
4. Truy cập `/Admin/User` để quản lý người dùng

### Breaking Changes
- ❌ Không còn `/Admin/Role/ManageUsers` - Chuyển sang `/Admin/User`
- ⚠️ Email không còn hiển thị trong console (cần cấu hình SMTP)

---

**Ghi chú:** Version 2.0 tập trung vào cải thiện trải nghiệm quản lý người dùng và email thực tế, giúp hệ thống sẵn sàng cho production.

