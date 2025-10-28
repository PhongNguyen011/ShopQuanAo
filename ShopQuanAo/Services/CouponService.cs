// Services/CouponService.cs
using Microsoft.EntityFrameworkCore;
using ShopQuanAo.Data;
using ShopQuanAo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopQuanAo.Services
{
    public class CouponService : ICouponService
    {
        private readonly ApplicationDbContext _db;
        public CouponService(ApplicationDbContext db) => _db = db;

        public async Task<(bool ok, string message, decimal discount, Coupon? coupon)>
            TryApplyAsync(string inputCode, IEnumerable<CartItem> cartItems)
        {
            var code = (inputCode ?? "").Trim().ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(code))
                return (false, "Vui lòng nhập mã.", 0m, null);

            var now = DateTime.UtcNow;
            var coupon = await _db.Coupons.FirstOrDefaultAsync(c => c.Code == code);
            if (coupon == null || !coupon.IsActive)
                return (false, "Mã không tồn tại hoặc đã bị vô hiệu.", 0m, null);

            if (coupon.StartDate > now || (coupon.EndDate.HasValue && coupon.EndDate < now))
                return (false, "Mã không còn hiệu lực.", 0m, coupon);

            var subtotal = cartItems.Sum(x => x.Price * x.Quantity);
            if (coupon.MinOrderAmount.HasValue && subtotal < coupon.MinOrderAmount.Value)
                return (false, $"Đơn tối thiểu {coupon.MinOrderAmount.Value:N0}đ.", 0m, coupon);

            if (coupon.Scope == CouponScope.CategoryOnly)
            {
                var ids = cartItems.Select(ci => ci.ProductId).Distinct().ToList();
                var categoriesInCart = await _db.Products
                    .Where(p => ids.Contains(p.Id))
                    .Select(p => p.Category)
                    .ToListAsync();

                var allowed = ParseCsv(coupon.AllowedCategoriesCsv);
                var matched = categoriesInCart.Any(cg =>
                    !string.IsNullOrWhiteSpace(cg) &&
                    allowed.Contains(cg.Trim(), StringComparer.OrdinalIgnoreCase));
                if (!matched)
                    return (false, "Mã không áp dụng cho danh mục trong giỏ.", 0m, coupon);
            }

            decimal discount = coupon.DiscountType == DiscountType.Percentage
                ? Math.Round(subtotal * (coupon.DiscountValue / 100m), 2, MidpointRounding.AwayFromZero)
                : coupon.DiscountValue;

            if (discount > subtotal) discount = subtotal;
            return (true, "Áp dụng mã thành công.", discount, coupon);
        }

        private static List<string> ParseCsv(string? csv)
            => string.IsNullOrWhiteSpace(csv)
                ? new()
                : csv.Split(',', StringSplitOptions.RemoveEmptyEntries)
                     .Select(s => s.Trim())
                     .Where(s => s.Length > 0)
                     .ToList();
    }
}
