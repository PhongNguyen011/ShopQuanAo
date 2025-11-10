# Hướng dẫn fix lỗi "Invalid column name 'ProvinceName'" và dropdown không có data

## Vấn đề 1: Lỗi SQL "Invalid column name 'ProvinceName'"

### Nguyên nhân:
Database chưa được cập nhật với cấu trúc mới (cột `ProvinceName` thay vì `ProvinceId`).

### Giải pháp:

**Cách 1: Chạy SQL Script (Nhanh nhất)**
1. Mở SQL Server Management Studio (SSMS) hoặc Azure Data Studio
2. Kết nối đến database `ShopQuanAoDB`
3. Mở file `ShopQuanAo/FIX_DATABASE.sql`
4. Chạy script (F5)
5. Restart ứng dụng

**Cách 2: Chạy Migration qua EF Core**
1. **Dừng ứng dụng** (quan trọng!)
2. Mở PowerShell hoặc Command Prompt
3. Chạy lệnh:
```bash
cd ShopQuanAo
dotnet ef database update
```
4. Restart ứng dụng

## Vấn đề 2: Dropdown không có data

### Nguyên nhân có thể:
1. GHN API Token không hợp lệ hoặc chưa được cấu hình
2. GHN API không trả về data
3. Network issue
4. Response format khác với expected

### Giải pháp:

**Bước 1: Kiểm tra GHN Token**
- Mở `appsettings.json`
- Kiểm tra `GHN:Token` có giá trị hợp lệ không
- Nếu token không hợp lệ, lấy token mới từ GHN

**Bước 2: Kiểm tra Console Log**
1. Mở trình duyệt (F12)
2. Vào tab Console
3. Reload trang checkout
4. Xem log để biết lỗi cụ thể:
   - Nếu thấy "GHN Token chưa được cấu hình" → Cập nhật token
   - Nếu thấy "HTTP error! status: XXX" → Kiểm tra GHN API
   - Nếu thấy "Invalid JSON response" → Kiểm tra response format

**Bước 3: Test API trực tiếp**
- Mở browser và truy cập: `https://localhost:7150/Checkout/GetProvinces`
- Xem response có đúng format không
- Nếu có lỗi, kiểm tra server console log

**Bước 4: Cấu hình GHN Token (nếu chưa có)**
1. Đăng ký tài khoản GHN tại: https://ghn.vn
2. Lấy API Token từ dashboard
3. Cập nhật vào `appsettings.json`:
```json
"GHN": {
    "BaseUrl": "https://online-gateway.ghn.vn/shiip/public-api/v2",
    "Token": "YOUR_TOKEN_HERE"
}
```

## Sau khi fix:

1. **Dừng ứng dụng**
2. **Chạy migration hoặc SQL script**
3. **Restart ứng dụng**
4. **Test lại:**
   - Vào Admin → Phí ship → Tạo phí ship cho một số tỉnh (ví dụ: "Thành phố Hồ Chí Minh", "Hà Nội")
   - Vào Checkout → Kiểm tra dropdown có load được tỉnh/quận/xã không
   - Chọn địa chỉ → Kiểm tra phí ship có được tính tự động không

## Lưu ý:

- Tên tỉnh/thành phố trong ShippingFee phải khớp CHÍNH XÁC với tên từ GHN API
- Ví dụ: "Thành phố Hồ Chí Minh" (không phải "Hồ Chí Minh" hay "TP.HCM")
- Để biết tên chính xác, check response từ API `/Checkout/GetProvinces`

## Troubleshooting:

### Nếu vẫn bị lỗi sau khi chạy script:
1. Kiểm tra xem table ShippingFees đã được tạo chưa:
```sql
SELECT * FROM ShippingFees
```

2. Kiểm tra cấu trúc table:
```sql
SELECT COLUMN_NAME, DATA_TYPE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'ShippingFees'
```

3. Nếu vẫn có cột `ProvinceId`, chạy lại script SQL

### Nếu dropdown vẫn không có data:
1. Kiểm tra Network tab trong browser DevTools
2. Xem request `/Checkout/GetProvinces` có được gửi không
3. Xem response có data không
4. Check server console log để xem lỗi từ GHN API

