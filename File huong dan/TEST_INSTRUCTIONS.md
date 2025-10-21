# 📝 HƯỚNG DẪN KIỂM TRA CHỨC NĂNG

## 🚀 Bước 1: Chạy Ứng Dụng

```bash
cd ShopQuanAo
dotnet run
```

Mở trình duyệt: `https://localhost:5001`

---

## ✅ KIỂM TRA TÍNH NĂNG EMAIL

### Test 1: Đăng Ký Tài Khoản Mới

1. Vào: `https://localhost:5001/Account/Register`
2. Điền thông tin:
   - Họ và tên: **Nguyễn Văn A**
   - Email: **test@example.com**
   - Số điện thoại: **0987654321**
   - Mật khẩu: **Test@123**
   - Xác nhận mật khẩu: **Test@123**
3. Click **Đăng Ký**

### ✨ KẾT QUẢ MONG ĐỢI:

**Trong Terminal/Console (nơi chạy dotnet run):**
```
====== EMAIL DEMO MODE ======
To: test@example.com
Subject: Xác nhận tài khoản của bạn
Body: [HTML email content with link]
=============================
```

**Trên Trang Đăng Nhập:**
- Thông báo: "Đăng ký thành công! Vui lòng kiểm tra email..."
- Hiển thị box màu xanh với **Link xác nhận (Demo Mode)**

### Test 2: Xác Nhận Email

1. Copy link từ console HOẶC click vào link trên trang đăng nhập
2. Link có dạng: `https://localhost:5001/Account/ConfirmEmail?userId=...&code=...`
3. Click vào link

### ✨ KẾT QUẢ MONG ĐỢI:
- Trang xác nhận: "Cảm ơn bạn đã xác nhận email. Bây giờ bạn có thể đăng nhập."

### Test 3: Đăng Nhập Tài Khoản Mới

1. Vào: `https://localhost:5001/Account/Login`
2. Đăng nhập với:
   - Email: **test@example.com**
   - Mật khẩu: **Test@123**
3. Click **Đăng Nhập**

### ✨ KẾT QUẢ MONG ĐỢI:
- Đăng nhập thành công
- Menu hiển thị tên người dùng và dropdown
- Không thấy "Admin Panel" trong menu (vì là User)

---

## 🔐 KIỂM TRA TÀI KHOẢN ADMIN CỐ ĐỊNH

### Test 4: Đăng Nhập Admin

1. Đăng xuất (nếu đang đăng nhập)
2. Vào: `https://localhost:5001/Account/Login`
3. Đăng nhập với:
   - Email: **admin@shopquanao.com**
   - Mật khẩu: **Admin@123**
4. Click **Đăng Nhập**

### ✨ KẾT QUẢ MONG ĐỢI:
- Tự động chuyển đến Admin Dashboard: `https://localhost:5001/Admin`
- Hiển thị thống kê: Tổng sản phẩm, Người dùng, v.v.
- Menu admin có: Dashboard, Sản Phẩm, Vai Trò

### Test 5: Kiểm Tra Admin Menu

**Trong Header (khi đăng nhập admin):**
- Click vào tên user → dropdown hiển thị
- Thấy menu item: **"Admin Panel"**
- Thấy menu: Thông Tin Cá Nhân, Đơn Hàng, Đăng Xuất

---

## 🛡️ KIỂM TRA PHÂN QUYỀN

### Test 6: User Không Vào Được Admin Area

1. Đăng nhập bằng tài khoản **test@example.com** (User)
2. Thử truy cập: `https://localhost:5001/Admin`

### ✨ KẾT QUẢ MONG ĐỢI:
- Chuyển đến trang: `https://localhost:5001/Account/AccessDenied`
- Hiển thị: "Bạn không có quyền truy cập vào trang này"

### Test 7: Admin Vào Được Mọi Trang

1. Đăng nhập bằng **admin@shopquanao.com**
2. Thử truy cập:
   - `https://localhost:5001/Admin` ✅
   - `https://localhost:5001/Admin/Product` ✅
   - `https://localhost:5001/Admin/Role` ✅

### ✨ KẾT QUẢ MONG ĐỢI:
- Tất cả đều truy cập được
- Không bị chặn

---

## 📧 KIỂM TRA QUÊN MẬT KHẨU

### Test 8: Quên Mật Khẩu

1. Đăng xuất
2. Vào: `https://localhost:5001/Account/ForgotPassword`
3. Nhập email: **test@example.com**
4. Click **Gửi liên kết đặt lại**

### ✨ KẾT QUẢ MONG ĐỢI:

**Trong Console:**
```
====== EMAIL DEMO MODE ======
To: test@example.com
Subject: Đặt lại mật khẩu
Body: [HTML with reset link]
=============================
```

5. Copy link từ console
6. Mở link trong trình duyệt
7. Nhập mật khẩu mới: **NewPass@123**
8. Xác nhận mật khẩu: **NewPass@123**
9. Click **Đặt Lại Mật Khẩu**

### ✨ KẾT QUẢ MONG ĐỢI:
- Thành công
- Đăng nhập lại với mật khẩu mới được

---

## 🎯 KIỂM TRA QUẢN LÝ ROLES (ADMIN)

### Test 9: Xem Danh Sách Roles

1. Đăng nhập admin
2. Vào: `https://localhost:5001/Admin/Role`

### ✨ KẾT QUẢ MONG ĐỢI:
- Hiển thị 2 roles: **Admin**, **User**
- Có nút "Tạo Vai Trò Mới"
- Mỗi role có nút "Quản lý người dùng"

### Test 10: Phân Quyền User

1. Click vào nút "Quản lý người dùng" của role **Admin**
2. Thấy danh sách users
3. Chọn user **test@example.com**
4. Click **Lưu Thay Đổi**
5. Đăng xuất và đăng nhập lại bằng **test@example.com**

### ✨ KẾT QUẢ MONG ĐỢI:
- User này giờ vào được Admin area
- Menu hiển thị "Admin Panel"

---

## 📊 KIỂM TRA ADMIN DASHBOARD

### Test 11: Dashboard Statistics

1. Đăng nhập admin
2. Vào: `https://localhost:5001/Admin`

### ✨ KẾT QUẢ MONG ĐỢI:
- Card 1: **Tổng Sản Phẩm** = 5
- Card 2: **Sản Phẩm Đang Bán** = 5
- Card 3: **Sản Phẩm Nổi Bật** = 3
- Card 4: **Người Dùng** = 2 (admin + test user)
- Hiển thị 2 card: Quản Lý Sản Phẩm, Quản Lý Vai Trò

---

## 🔒 KIỂM TRA GHI NHỚ TÀI KHOẢN

### Test 12: Remember Me

1. Đăng xuất
2. Đăng nhập với admin
3. ✅ **Check** vào "Ghi nhớ tài khoản"
4. Đăng nhập
5. Đóng trình duyệt
6. Mở lại trình duyệt
7. Vào `https://localhost:5001`

### ✨ KẾT QUẢ MONG ĐỢI:
- Vẫn đăng nhập
- Không cần đăng nhập lại

---

## 🎨 KIỂM TRA UI/UX

### Test 13: Responsive Design

1. Mở DevTools (F12)
2. Toggle device toolbar
3. Test trên mobile, tablet, desktop

### ✨ KẾT QUẢ MONG ĐỢI:
- Layout responsive
- Menu collapse trên mobile
- Form hiển thị đúng

---

## 🐛 KIỂM TRA XỬ LÝ LỖI

### Test 14: Email Đã Tồn Tại

1. Đăng ký với email: **admin@shopquanao.com**

### ✨ KẾT QUẢ MONG ĐỢI:
- Lỗi: Email đã được sử dụng

### Test 15: Đăng Nhập Chưa Xác Nhận Email

1. Tạo user mới nhưng KHÔNG xác nhận email
2. Thử đăng nhập

### ✨ KẾT QUẢ MONG ĐỢI:
- Lỗi: "Vui lòng xác nhận email trước khi đăng nhập"

### Test 16: Mật Khẩu Sai 5 Lần

1. Đăng nhập với mật khẩu sai 5 lần

### ✨ KẾT QUẢ MONG ĐỢI:
- Lần thứ 6: "Tài khoản đã bị khóa tạm thời do đăng nhập sai quá nhiều lần"

---

## 📋 CHECKLIST TỔNG KẾT

- [ ] Đăng ký tài khoản mới hiển thị link trong console
- [ ] Xác nhận email hoạt động
- [ ] Đăng nhập thành công sau xác nhận
- [ ] Admin account hoạt động: admin@shopquanao.com / Admin@123
- [ ] User không vào được Admin area
- [ ] Admin vào được Admin area
- [ ] Quên mật khẩu hoạt động
- [ ] Ghi nhớ tài khoản hoạt động
- [ ] Dashboard hiển thị thống kê đúng
- [ ] Quản lý roles hoạt động
- [ ] Phân quyền user hoạt động
- [ ] UI responsive

---

## 📞 NẾU CÓ LỖI

1. Kiểm tra console/terminal output
2. Kiểm tra browser console (F12)
3. Đọc file `HUONG_DAN_CAU_HINH.md`
4. Database có được tạo chưa: SQL Server Object Explorer → mssqllocaldb → ShopQuanAoDB



