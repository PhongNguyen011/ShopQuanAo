# HƯỚNG DẪN CÀI ĐẶT VÀ CẤU HÌNH

## 📋 Tài Khoản Admin Mặc Định

Tài khoản admin đã được tạo sẵn trong database:

```
Email: admin@shopquanao.com
Mật khẩu: Admin@123
```

**Lưu ý:** Tài khoản này đã được xác nhận email và có thể đăng nhập ngay lập tức.

## 🚀 Cách Chạy Ứng Dụng

### 1. Khởi động ứng dụng

```bash
cd ShopQuanAo
dotnet run
```

### 2. Truy cập

- **Trang chủ:** https://localhost:5001 hoặc http://localhost:5000
- **Admin Panel:** https://localhost:5001/Admin
- **Đăng nhập:** https://localhost:5001/Account/Login

## 📧 Cấu Hình Email (Tùy Chọn)

### Chế Độ Demo (Mặc Định)

Hiện tại ứng dụng đang chạy ở **chế độ demo**. Khi đăng ký tài khoản mới:

1. Email **KHÔNG** được gửi thật
2. Link xác nhận sẽ hiển thị trong:
   - Console/Terminal (khi chạy ứng dụng)
   - Trang đăng nhập (sau khi đăng ký)
3. Click vào link để xác nhận tài khoản

### Cấu Hình Email Thật (Gmail)

Nếu bạn muốn gửi email thật, cập nhật file `appsettings.json`:

```json
"EmailSettings": {
  "SenderEmail": "your-email@gmail.com",
  "Password": "your-app-password",
  "SmtpServer": "smtp.gmail.com",
  "Port": "587",
  "SenderName": "Shop Quần Áo"
}
```

### Hướng Dẫn Lấy App Password Cho Gmail:

1. Truy cập: https://myaccount.google.com/
2. Chọn **Security** (Bảo mật)
3. Bật **2-Step Verification** (Xác minh 2 bước) nếu chưa bật
4. Tìm **App passwords** (Mật khẩu ứng dụng)
5. Chọn **Mail** và **Windows Computer**
6. Click **Generate** (Tạo)
7. Copy mật khẩu 16 ký tự và dán vào `appsettings.json`

**Lưu ý:** 
- Không dùng mật khẩu Gmail thông thường
- Phải bật xác minh 2 bước mới có App Password
- Giữ bí mật App Password

### Các Nhà Cung Cấp Email Khác

**Outlook/Hotmail:**
```json
"SmtpServer": "smtp-mail.outlook.com",
"Port": "587"
```

**Yahoo:**
```json
"SmtpServer": "smtp.mail.yahoo.com",
"Port": "587"
```

## 🔐 Chức Năng Đã Triển Khai

### 1. Xác Thực & Phân Quyền
- ✅ Đăng ký tài khoản với xác nhận email
- ✅ Đăng nhập với "Ghi nhớ tài khoản"
- ✅ Quên mật khẩu & Đặt lại mật khẩu
- ✅ Phân quyền Admin/User
- ✅ Chặn User truy cập Admin Area

### 2. Admin Panel (Chỉ Admin)
- ✅ Dashboard với thống kê
- ✅ Quản lý sản phẩm (CRUD)
- ✅ Quản lý vai trò (Roles)
- ✅ Phân quyền người dùng

### 3. Tính Năng User
- ✅ Xem danh sách sản phẩm
- ✅ Xem chi tiết sản phẩm
- ✅ Menu người dùng đã đăng nhập

## 🛠️ Database

Database đã được tạo tự động với:
- 5 sản phẩm mẫu
- 2 roles: Admin, User
- 1 tài khoản admin (xem phía trên)

Để reset database:
```bash
dotnet ef database drop --force
dotnet ef database update
```

## 📝 Luồng Đăng Ký Tài Khoản

### Chế Độ Demo (Không cấu hình email):
1. Người dùng điền form đăng ký
2. Hệ thống tạo tài khoản
3. Link xác nhận hiển thị trong console và trang đăng nhập
4. Click link để xác nhận → Có thể đăng nhập

### Chế Độ Email Thật (Đã cấu hình):
1. Người dùng điền form đăng ký
2. Hệ thống tạo tài khoản
3. Email xác nhận được gửi đến hộp thư
4. Click link trong email → Có thể đăng nhập

## ⚠️ Lưu Ý Quan Trọng

1. **Yêu Cầu Xác Nhận Email:** Tài khoản phải được xác nhận email mới đăng nhập được (trừ admin)
2. **Khóa Tài Khoản:** Đăng nhập sai 5 lần sẽ bị khóa 5 phút
3. **Mật Khẩu Mạnh:** Ít nhất 6 ký tự, có chữ hoa, chữ thường, số
4. **Port:** Đảm bảo port 5000/5001 không bị chiếm bởi ứng dụng khác

## 🐛 Xử Lý Lỗi Thường Gặp

### Lỗi: "Email already exists"
→ Email đã được đăng ký, dùng email khác hoặc đăng nhập

### Lỗi: "Email not confirmed"
→ Click link xác nhận trong console hoặc email

### Lỗi: "Access Denied"
→ Tài khoản User không thể vào Admin Area

### Không nhận được email
→ Kiểm tra:
- Console/Terminal có hiển thị link không
- Cấu hình email trong appsettings.json
- Gmail App Password đúng chưa
- Spam folder

## 📞 Hỗ Trợ

Nếu gặp vấn đề, kiểm tra:
1. Console output khi chạy ứng dụng
2. Browser console (F12)
3. Database đã được tạo chưa

## 🔄 Update Code

Sau khi update code, chạy:
```bash
dotnet clean
dotnet build
dotnet run
```

Nếu có thay đổi database:
```bash
dotnet ef migrations add TenMigration
dotnet ef database update
```



