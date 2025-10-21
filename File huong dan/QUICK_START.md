# ⚡ Quick Start Guide

## 🚀 Chạy Ngay (1 phút)

```bash
# 1. Restore packages
dotnet restore

# 2. Update database
dotnet ef database update

# 3. Run
dotnet run
```

Truy cập: `https://localhost:xxxx`

---

## 🎯 Tính Năng Mới (Version 2.1)

### ✅ Email Thông Minh
- **Chưa cấu hình:** Nút "Xác nhận ngay" trên trang Login
- **Đã cấu hình:** Gửi email tự động

### ✨ Trang Login/Register Mới
- Argon Dashboard style
- Full-screen đẹp
- Responsive

### 👤 Profile Admin Đẹp
- Dropdown với avatar
- Thông tin đầy đủ
- Nút logout nổi bật

### ✔️ Checkbox Vai Trò Rõ Ràng
- Box lớn, dễ click
- Visual feedback
- Icon check

---

## 📧 Cấu Hình Email (5 phút)

### Bước 1: Tạo App Password
1. Vào: https://myaccount.google.com/security
2. Bật "2-Step Verification"
3. Tạo "App Password"
4. Copy 16 ký tự

### Bước 2: Cập Nhật appsettings.json
```json
{
  "EmailSettings": {
    "SenderEmail": "your-email@gmail.com",
    "Password": "abcd1234efgh5678",
    "SmtpServer": "smtp.gmail.com",
    "Port": "587"
  }
}
```

### Bước 3: Test
- Đăng ký tài khoản mới
- Kiểm tra email
- Click link xác thực

---

## 🧪 Testing Nhanh (Không cần email)

1. Đăng ký tài khoản
2. Vào trang Login
3. Thấy nút "Xác nhận ngay"
4. Click → Done!

---

## 🎨 Trải Nghiệm

### Trang Chủ
- Nút "Đăng Nhập" + "Đăng Ký" ở góc phải
- Click vào để xem trang mới đẹp

### Trang Login
- Full-screen với background
- Form bên trái
- Gradient image bên phải

### Trang Register
- Card gradient đẹp
- Validation real-time
- Password requirements

### Admin Panel
- Click avatar góc phải để xem profile
- Dropdown với thông tin đầy đủ
- Logout nhanh

---

## 📖 Tài Liệu Đầy Đủ

- **Tổng quát:** `HUONG_DAN_SU_DUNG.md`
- **Email chi tiết:** `HUONG_DAN_CAU_HINH_EMAIL_CHI_TIET.md`
- **UI improvements:** `CAI_TIEN_UI.md`
- **Thông báo mới:** `THONG_BAO_CAI_TIEN_MOI.md`
- **Changelog:** `CHANGELOG.md`

---

## ⚙️ Tài Khoản Mặc Định

### Admin
- Email: `admin@shopquanao.com`
- Password: `Admin@123`

### User
- Email: `user@shopquanao.com`
- Password: `User@123`

---

## 🐛 Gặp Vấn Đề?

1. Xem console logs
2. Check `HUONG_DAN_CAU_HINH_EMAIL_CHI_TIET.md`
3. Xem Spam folder
4. Thử mode testing (không cần email)

---

**Happy Coding! 🎉**

