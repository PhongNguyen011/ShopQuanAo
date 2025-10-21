# Hướng Dẫn Cấu Hình Email Chi Tiết

## 🎯 Tổng Quan

Hiện tại hệ thống có **2 chế độ hoạt động**:

### 📧 Mode 1: Email Thật (Khuyến nghị cho Production)
- Gửi email xác thực đến hộp thư người dùng
- Sử dụng Gmail SMTP
- Cần cấu hình App Password

### 🔗 Mode 2: Link Trực Tiếp (Cho Testing)
- Không cần cấu hình email
- Hiển thị nút "Xác nhận ngay" trên trang Login
- Phù hợp cho development/testing

---

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

### Bước 3: Test

1. Chạy ứng dụng:
```bash
dotnet run
```

2. Đăng ký tài khoản mới
3. Kiểm tra email (Inbox hoặc Spam)
4. Click link trong email

✅ **Thành công** nếu nhận được email!

---

## 🔍 Cách Biết Email Đã Được Cấu Hình

### Sau khi đăng ký:

#### ✅ Email Đã Cấu Hình:
```
Thông báo màu xanh:
"Đăng ký thành công! Vui lòng kiểm tra email để xác nhận tài khoản."
```

#### ⚠️ Email Chưa Cấu Hình:
```
Thông báo màu vàng:
"Đăng ký thành công! Email chưa được cấu hình. 
Vui lòng click link bên dưới để xác nhận tài khoản."

[Nút: Xác nhận ngay]  ← Click để xác thực luôn
```

---

## 📝 Ví Dụ Cấu Hình Thật

### ✅ Đúng
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

### ❌ Sai
```json
{
  "EmailSettings": {
    "SenderEmail": "pntshop@gmail.com",
    "Password": "abcd efgh ijkl mnop",    // ❌ Có khoảng trắng
    "SmtpServer": "smtp.gmail.com",
    "Port": "587"
  }
}
```

---

## 🐛 Troubleshooting

### Lỗi 1: "Username and Password not accepted"

**Nguyên nhân:**
- Sai email hoặc App Password
- Chưa bật 2-Step Verification
- App Password có khoảng trắng

**Giải pháp:**
1. Kiểm tra email chính xác
2. Đảm bảo đã bật 2-Step Verification
3. Tạo lại App Password
4. Copy chính xác 16 ký tự, **không có khoảng trắng**

### Lỗi 2: Không nhận được email

**Kiểm tra:**
1. ✅ Inbox
2. ✅ Spam/Junk folder
3. ✅ Promotions tab (Gmail)
4. ✅ Console log có thông báo "✓ Email đã được gửi thành công"

**Nếu không thấy email:**
- Đợi 1-2 phút (có thể delay)
- Kiểm tra email đúng không
- Thử gửi lại bằng cách đăng ký email khác

### Lỗi 3: "Unable to connect to SMTP server"

**Nguyên nhân:**
- Firewall/Antivirus chặn port 587
- Không có Internet
- SMTP Server sai

**Giải pháp:**
1. Tắt Firewall/Antivirus tạm thời để test
2. Kiểm tra kết nối Internet
3. Đảm bảo `SmtpServer` = `smtp.gmail.com`
4. Đảm bảo `Port` = `587`

### Lỗi 4: Email vào Spam

**Nguyên nhân:** 
- Email mới, chưa có reputation
- Nội dung có từ khóa spam

**Giải pháp:**
1. Check Spam folder
2. Đánh dấu "Not Spam"
3. Add email sender vào Contacts
4. Lần sau sẽ vào Inbox

---

## 🔒 Bảo Mật

### ⚠️ QUAN TRỌNG

**KHÔNG BAO GIỜ:**
- Commit file `appsettings.json` với thông tin email thật lên GitHub
- Share App Password với ai
- Sử dụng password thật (phải dùng App Password)

### ✅ Best Practices

#### Development (Local)

Sử dụng **User Secrets**:

```bash
cd ShopQuanAo
dotnet user-secrets init
dotnet user-secrets set "EmailSettings:SenderEmail" "your-email@gmail.com"
dotnet user-secrets set "EmailSettings:Password" "your-app-password"
```

File secrets sẽ lưu ở:
- Windows: `%APPDATA%\Microsoft\UserSecrets\<user_secrets_id>\secrets.json`
- Mac/Linux: `~/.microsoft/usersecrets/<user_secrets_id>/secrets.json`

**Không bao giờ** bị commit lên Git!

#### Production (Server)

Sử dụng **Environment Variables**:

**Azure App Service:**
```
Configuration > Application settings
- EmailSettings__SenderEmail = your-email@gmail.com
- EmailSettings__Password = your-app-password
```

**IIS:**
```xml
<environmentVariables>
  <environmentVariable name="EmailSettings__SenderEmail" value="your-email@gmail.com" />
  <environmentVariable name="EmailSettings__Password" value="your-app-password" />
</environmentVariables>
```

---

## 📊 Giám Sát Email

### Console Logs

Khi chạy app, quan sát console:

#### ✅ Thành công:
```
✓ Email đã được gửi thành công đến user@example.com
```

#### ⚠️ Chưa cấu hình:
```
==================================================
CẢNH BÁO: Cấu hình email chưa được thiết lập!
Vui lòng cập nhật EmailSettings trong appsettings.json:
...
==================================================
```

#### ❌ Lỗi:
```
✗ Lỗi khi gửi email đến user@example.com
Chi tiết lỗi: ...
```

---

## 🧪 Testing

### Test 1: Cấu hình đúng

1. Cấu hình email đúng
2. Đăng ký tài khoản mới
3. Kiểm tra console: `✓ Email đã được gửi thành công`
4. Kiểm tra email inbox
5. Click link xác thực

### Test 2: Chưa cấu hình

1. Để `appsettings.json` với giá trị mặc định
2. Đăng ký tài khoản mới
3. Thấy thông báo vàng + nút "Xác nhận ngay"
4. Click nút để xác thực

### Test 3: Reset Password

1. Login page > "Quên mật khẩu?"
2. Nhập email
3. Kiểm tra email reset password
4. Click link và đặt mật khẩu mới

---

## 📧 Template Email

Hệ thống gửi email với template HTML đẹp:

### Email Xác Thực Tài Khoản
- Logo/Header với màu primary
- Lời chào cá nhân hóa
- Nút "Xác nhận email" lớn, rõ ràng
- Link dự phòng nếu nút không hoạt động
- Footer với disclaimer

### Email Reset Password
- Cảnh báo bảo mật
- Nút "Đặt lại mật khẩu" màu đỏ
- Thời gian hết hạn
- Hướng dẫn nếu không yêu cầu

---

## 🚀 Production Checklist

Trước khi deploy:

- [ ] Đã tạo App Password từ Gmail
- [ ] Đã test gửi email thành công local
- [ ] Đã setup User Secrets hoặc Environment Variables
- [ ] **KHÔNG** commit `appsettings.json` với thông tin thật
- [ ] Đã add email sender vào whitelist (nếu có)
- [ ] Đã test trên production environment
- [ ] Đã kiểm tra Spam folder
- [ ] Đã setup monitoring/logging

---

## ❓ FAQ

**Q: Có thể dùng email khác Gmail không?**  
A: Có! Thay đổi `SmtpServer` và `Port`:
- **Outlook:** `smtp.office365.com`, port `587`
- **Yahoo:** `smtp.mail.yahoo.com`, port `587`
- **Custom SMTP:** Theo hướng dẫn provider

**Q: App Password có hết hạn không?**  
A: Không, trừ khi bạn revoke hoặc tắt 2-Step Verification.

**Q: Có giới hạn số email gửi không?**  
A: Gmail free có giới hạn ~500 emails/day.

**Q: Làm sao để tắt chế độ demo?**  
A: Không cần tắt! Hệ thống tự động chuyển sang gửi email thật khi cấu hình đúng.

**Q: Email bị reject/bounce?**  
A: 
- Kiểm tra email người nhận đúng
- Email có tồn tại không
- Mailbox có full không

---

## 📞 Support

Nếu vẫn gặp vấn đề:

1. Xem lại toàn bộ hướng dẫn này
2. Check console logs
3. Check email Spam folder
4. Thử tạo lại App Password
5. Test với email khác

---

**Version:** 2.1  
**Updated:** 2025-10-21

