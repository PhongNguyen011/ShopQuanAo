# Hướng Dẫn Cấu Hình Email

## Tổng Quan
Hệ thống sử dụng Gmail SMTP để gửi email xác thực tài khoản và khôi phục mật khẩu.

## Các Bước Cấu Hình

### 1. Tạo App Password cho Gmail

#### Bước 1: Bật 2-Step Verification
1. Truy cập https://myaccount.google.com/security
2. Tìm và bật **2-Step Verification** (Xác minh 2 bước)
3. Làm theo hướng dẫn để hoàn tất thiết lập

#### Bước 2: Tạo App Password
1. Vào https://myaccount.google.com/apppasswords
2. Đăng nhập nếu được yêu cầu
3. Chọn **Select app** → chọn **Other (Custom name)**
4. Nhập tên: `ShopQuanAo` hoặc tên bất kỳ
5. Nhấn **Generate**
6. Google sẽ hiển thị mật khẩu 16 ký tự (ví dụ: `abcd efgh ijkl mnop`)
7. **Sao chép mật khẩu này** (bỏ khoảng trắng nếu có)

### 2. Cấu Hình trong appsettings.json

Mở file `ShopQuanAo/appsettings.json` và cập nhật phần `EmailSettings`:

```json
{
  "EmailSettings": {
    "SenderEmail": "your-email@gmail.com",        // ← Thay bằng email Gmail của bạn
    "Password": "abcdefghijklmnop",               // ← Thay bằng App Password (16 ký tự, không có khoảng trắng)
    "SmtpServer": "smtp.gmail.com",               // Giữ nguyên
    "Port": "587",                                 // Giữ nguyên
    "SenderName": "Shop Quần Áo"                   // Tên hiển thị khi gửi email
  }
}
```

### Ví Dụ Cấu Hình Thực Tế

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

## Kiểm Tra Cấu Hình

### 1. Chạy ứng dụng
```bash
dotnet run --project ShopQuanAo
```

### 2. Thử đăng ký tài khoản mới
- Truy cập: `/Account/Register`
- Điền thông tin và đăng ký
- Kiểm tra email (Inbox hoặc Spam/Junk)

### 3. Xem Log
Nếu email được cấu hình đúng, trong console sẽ hiển thị:
```
✓ Email đã được gửi thành công đến <email>
```

Nếu cấu hình chưa đúng:
```
⚠ CẢNH BÁO: Cấu hình email chưa được thiết lập!
```

## Xử Lý Lỗi Thường Gặp

### Lỗi: "Username and Password not accepted"
**Nguyên nhân:** Sai email hoặc App Password

**Giải pháp:**
- Kiểm tra lại email Gmail
- Kiểm tra App Password (16 ký tự, không có khoảng trắng)
- Đảm bảo đã bật 2-Step Verification

### Lỗi: "Unable to connect to SMTP server"
**Nguyên nhân:** Không kết nối được đến server

**Giải pháp:**
- Kiểm tra kết nối Internet
- Kiểm tra Firewall/Antivirus có chặn port 587 không
- Thử đổi Port sang `465` và SmtpServer sang `SecureSocketOptions.SslOnConnect`

### Email vào Spam
**Giải pháp:**
- Kiểm tra Spam/Junk folder
- Đánh dấu "Not Spam" để lần sau vào Inbox
- Thêm email gửi vào danh bạ

### Không nhận được email
**Kiểm tra:**
1. Email có đúng không?
2. Kiểm tra cả Inbox và Spam
3. Xem log trong Console có thông báo gửi thành công không
4. Thử gửi lại bằng cách đăng ký tài khoản khác

## Bảo Mật

### ⚠️ QUAN TRỌNG
- **KHÔNG** commit file `appsettings.json` có chứa thông tin email thật lên GitHub
- Sử dụng `appsettings.Development.json` cho môi trường local
- Sử dụng User Secrets hoặc Environment Variables cho production

### Sử dụng User Secrets (Khuyến nghị)

1. Khởi tạo User Secrets:
```bash
cd ShopQuanAo
dotnet user-secrets init
```

2. Thêm cấu hình email:
```bash
dotnet user-secrets set "EmailSettings:SenderEmail" "your-email@gmail.com"
dotnet user-secrets set "EmailSettings:Password" "your-app-password"
```

3. Các secrets này sẽ được lưu riêng, không bị commit lên Git

## Môi Trường Production

Với Azure App Service, AWS, hoặc hosting khác:
1. Vào phần Configuration/Environment Variables
2. Thêm các biến:
   - `EmailSettings__SenderEmail`
   - `EmailSettings__Password`
   - `EmailSettings__SmtpServer`
   - `EmailSettings__Port`

## Hỗ Trợ

Nếu gặp vấn đề, vui lòng:
1. Kiểm tra log trong Console
2. Kiểm tra lại các bước cấu hình
3. Tham khảo: https://support.google.com/accounts/answer/185833

---

**Lưu ý:** App Password chỉ hoạt động khi đã bật 2-Step Verification cho tài khoản Google.

