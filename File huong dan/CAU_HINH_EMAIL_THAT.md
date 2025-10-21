# 📧 Cấu Hình Email Thật - Gửi Đến Gmail

## 🎯 Mục Tiêu
Gửi email xác thực thật đến Gmail khi đăng ký tài khoản.

## ⚡ Cấu Hình Nhanh (5 phút)

### Bước 1: Tạo App Password từ Google

1. **Truy cập:** https://myaccount.google.com/security

2. **Bật 2-Step Verification** (nếu chưa có)
   - Click "2-Step Verification"
   - Follow hướng dẫn để bật

3. **Tạo App Password**
   - Sau khi bật 2-Step, quay lại Security
   - Tìm "App passwords" (có thể cần search)
   - Click "App passwords"
   - Chọn "Other (Custom name)"
   - Nhập tên: `ShopQuanAo`
   - Click "Generate"
   - **Copy 16 ký tự** (ví dụ: `abcd efgh ijkl mnop`)
   - **Quan trọng:** Bỏ khoảng trắng khi paste

### Bước 2: Cập Nhật appsettings.json

Mở file `ShopQuanAo/appsettings.json`:

```json
{
  "EmailSettings": {
    "SenderEmail": "your-email@gmail.com",      // ← Thay bằng Gmail của bạn
    "Password": "abcdefghijklmnop",             // ← Paste App Password (16 ký tự, không khoảng trắng)
    "SmtpServer": "smtp.gmail.com",             // Giữ nguyên
    "Port": "587",                               // Giữ nguyên
    "SenderName": "Shop Quần Áo"                 // Tên hiển thị
  }
}
```

### Ví Dụ Thật:
```json
{
  "EmailSettings": {
    "SenderEmail": "pntshop2024@gmail.com",
    "Password": "abcd1234efgh5678",
    "SmtpServer": "smtp.gmail.com",
    "Port": "587",
    "SenderName": "PNT Shop"
  }
}
```

### Bước 3: Test

1. **Chạy ứng dụng:**
```bash
dotnet run
```

2. **Đăng ký tài khoản mới**
3. **Kiểm tra email** (Inbox hoặc Spam)
4. **Click link trong email**

## ✅ Kết Quả Mong Đợi

### Console Log:
```
✓ Email đã được gửi thành công đến user@gmail.com
```

### Email Nhận Được:
- **From:** Shop Quần Áo <your-email@gmail.com>
- **Subject:** Xác nhận tài khoản của bạn
- **Content:** HTML đẹp với nút "Xác nhận email"
- **Link:** Click để kích hoạt tài khoản

## 🐛 Troubleshooting

### Lỗi: "Username and Password not accepted"
**Giải pháp:**
1. Kiểm tra email chính xác
2. Đảm bảo đã bật 2-Step Verification
3. Tạo lại App Password
4. Copy chính xác 16 ký tự, **không có khoảng trắng**

### Không nhận được email
**Kiểm tra:**
1. ✅ Inbox
2. ✅ Spam/Junk folder
3. ✅ Promotions tab (Gmail)
4. ✅ Console log có thông báo "✓ Email đã được gửi thành công"

### Email vào Spam
**Giải pháp:**
1. Check Spam folder
2. Đánh dấu "Not Spam"
3. Add email sender vào Contacts

## 🔒 Bảo Mật

### ⚠️ QUAN TRỌNG
- **KHÔNG** commit file `appsettings.json` với thông tin email thật lên GitHub
- Sử dụng **User Secrets** cho development
- Sử dụng **Environment Variables** cho production

### User Secrets (Khuyến nghị):
```bash
cd ShopQuanAo
dotnet user-secrets init
dotnet user-secrets set "EmailSettings:SenderEmail" "your-email@gmail.com"
dotnet user-secrets set "EmailSettings:Password" "your-app-password"
```

## 📊 So Sánh

| Trước | Sau |
|-------|-----|
| ❌ Demo mode (console log) | ✅ Email thật đến Gmail |
| ❌ Nút "Xác nhận ngay" | ✅ Link trong email |
| ❌ Không cần cấu hình | ✅ Cần App Password |

---

**Sau khi cấu hình xong, hệ thống sẽ tự động gửi email thật đến Gmail! 📧✨**

