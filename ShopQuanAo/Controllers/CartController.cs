using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopQuanAo.Data;
using ShopQuanAo.Models;
using ShopQuanAo.Services;
using ShopQuanAo.Utils; // SessionExtensions (SetObject/GetObject)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopQuanAo.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ICartService _cart;
        private readonly ICouponService _coupon;

        public CartController(ApplicationDbContext db, ICartService cart, ICouponService coupon)
        {
            _db = db;
            _cart = cart;
            _coupon = coupon;
        }

        private const string COUPON_KEY = "CART_COUPON";

        // Info coupon áp dụng cho GIỎ (order-level), lưu session
        public class AppliedCouponInfo
        {
            public string Code { get; set; } = "";
            public decimal Discount { get; set; }           // Số tiền giảm đã tính cho giỏ hiện tại
            public string Message { get; set; } = "";

            // Meta cho View (không ảnh hưởng DB CartItem)
            public ShopQuanAo.Models.DiscountType DiscountType { get; set; }
            public decimal DiscountValue { get; set; }
            public decimal? MinOrderAmount { get; set; }
            public ShopQuanAo.Models.CouponScope Scope { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public bool IsActive { get; set; }
            public string? AllowedCategoriesCsv { get; set; }
        }

        private AppliedCouponInfo? GetAppliedCoupon()
            => HttpContext.Session.GetObject<AppliedCouponInfo>(COUPON_KEY);

        private void SetAppliedCoupon(AppliedCouponInfo? info)
        {
            if (info == null) HttpContext.Session.Remove(COUPON_KEY);
            else HttpContext.Session.SetObject(COUPON_KEY, info);
        }

        // ====== PAGES ======

        public async Task<IActionResult> Index()
        {
            var items = _cart.GetCart();

            var ids = items.Select(x => x.ProductId).ToList();
            var stockMap = await _db.Products
                                    .Where(p => ids.Contains(p.Id))
                                    .Select(p => new { p.Id, p.StockQuantity })
                                    .ToDictionaryAsync(x => x.Id, x => x.StockQuantity);

            var (subtotal, discount, total) = await RecalcTotalsWithCouponAsync(items);

            ViewBag.StockMap = stockMap;
            ViewBag.Subtotal = subtotal;
            ViewBag.Discount = discount;
            ViewBag.Total = total;
            ViewBag.AppliedCoupon = GetAppliedCoupon();
            ViewBag.Message = TempData["CartMessage"];

            return View(items);
        }

        // ====== CART OPS ======

        // GET/POST /Cart/Add/5?qty=1
        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> Add(int id, int qty = 1)
        {
            if (qty < 1) qty = 1;

            var p = await _db.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (p == null) return NotFound();

            var cart = _cart.GetCart();
            var line = cart.FirstOrDefault(x => x.ProductId == id);

            var stock = p.StockQuantity;
            if (stock <= 0)
            {
                TempData["CartMessage"] = $"Sản phẩm “{p.Name}” đã hết hàng.";
                return RedirectToAction(nameof(Index));
            }

            if (line == null)
            {
                var addQty = Math.Min(qty, stock);
                cart.Add(new CartItem
                {
                    ProductId = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Image = p.ImageUrl,
                    Quantity = addQty
                });
            }
            else
            {
                line.Quantity = Math.Min(line.Quantity + qty, stock);
            }

            _cart.SaveCart(cart);
            TempData["CartMessage"] = $"Đã thêm “{p.Name}” vào giỏ.";
            return RedirectToAction(nameof(Index));
        }

        // Nút Cập nhật (form toàn bộ giỏ)
        // View: <form asp-action="Update" method="post"> với cặp ids[] / qty[]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int[] ids, int[] qty)
        {
            var cart = _cart.GetCart();
            if (ids == null || qty == null || ids.Length != qty.Length)
            {
                TempData["CartMessage"] = "Dữ liệu cập nhật không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            var desired = ids.Zip(qty, (id, q) => new { id, q = Math.Max(1, q) })
                             .ToDictionary(x => x.id, x => x.q);

            var idList = desired.Keys.ToList();
            var stockMap = await _db.Products
                                    .Where(p => idList.Contains(p.Id))
                                    .Select(p => new { p.Id, p.StockQuantity })
                                    .ToDictionaryAsync(x => x.Id, x => x.StockQuantity);

            foreach (var line in cart.ToList())
            {
                if (!desired.TryGetValue(line.ProductId, out var want))
                    continue;

                var stock = stockMap.TryGetValue(line.ProductId, out var s) ? s : 0;
                if (stock <= 0)
                {
                    cart.RemoveAll(x => x.ProductId == line.ProductId);
                    continue;
                }

                line.Quantity = Math.Min(Math.Max(1, want), stock);
            }

            _cart.SaveCart(cart);
            await RecalcTotalsWithCouponAsync(cart);
            TempData["CartMessage"] = "Đã cập nhật giỏ hàng.";
            return RedirectToAction(nameof(Index));
        }

        // AJAX: +/- từng dòng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeQty(int id, int delta)
        {
            var cart = _cart.GetCart();
            var line = cart.FirstOrDefault(x => x.ProductId == id);
            if (line == null)
            {
                return Json(new { ok = false, error = "Item not found" });
            }

            var stock = await _db.Products.Where(p => p.Id == id)
                                          .Select(p => p.StockQuantity)
                                          .FirstOrDefaultAsync();

            if (stock <= 0)
            {
                cart.RemoveAll(x => x.ProductId == id);
                _cart.SaveCart(cart);

                var (subtotal0, discount0, total0) = await RecalcTotalsWithCouponAsync(cart);
                return Json(new { ok = true, removed = true, subtotal = subtotal0, discount = discount0, total = total0 });
            }

            var newQty = line.Quantity + delta;
            if (newQty < 1) newQty = 1;
            if (newQty > stock) newQty = stock;

            line.Quantity = newQty;
            _cart.SaveCart(cart);

            var (subtotal, discount, total) = await RecalcTotalsWithCouponAsync(cart);

            return Json(new
            {
                ok = true,
                qty = line.Quantity,
                lineTotal = line.LineTotal,
                subtotal,
                discount,
                total,
                stock = stock,
                maxed = line.Quantity >= stock,
                mined = line.Quantity <= 1
            });
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int id)
        {
            var cart = _cart.GetCart();
            cart.RemoveAll(x => x.ProductId == id);
            _cart.SaveCart(cart);

            await RecalcTotalsWithCouponAsync(cart);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Clear()
        {
            _cart.SaveCart(new List<CartItem>());
            SetAppliedCoupon(null);
            return RedirectToAction(nameof(Index));
        }

        // ====== COUPON ======

        // Hỗ trợ nhập nhiều mã: "CODE1, CODE2" -> chọn mã giảm nhiều nhất
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApplyCoupon(string code)
        {
            var cart = _cart.GetCart();
            if (cart.Count == 0)
            {
                SetAppliedCoupon(null);
                TempData["CartMessage"] = "Giỏ hàng trống.";
                return RedirectToAction(nameof(Index));
            }

            if (string.IsNullOrWhiteSpace(code))
            {
                SetAppliedCoupon(null);
                TempData["CartMessage"] = "Vui lòng nhập mã.";
                return RedirectToAction(nameof(Index));
            }

            var codes = code.Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => s.Trim().ToUpperInvariant())
                            .Distinct()
                            .ToList();

            AppliedCouponInfo? best = null;
            decimal bestDiscount = 0m;
            string bestMessage = "Không có mã hợp lệ.";

            foreach (var c in codes)
            {
                var re = await _coupon.TryApplyAsync(c, cart);
                if (re.ok && re.coupon != null && re.discount > bestDiscount)
                {
                    bestDiscount = re.discount;
                    bestMessage = re.message;

                    best = new AppliedCouponInfo
                    {
                        Code = re.coupon.Code,
                        Discount = re.discount,
                        Message = re.message,
                        DiscountType = re.coupon.DiscountType,
                        DiscountValue = re.coupon.DiscountValue,
                        MinOrderAmount = re.coupon.MinOrderAmount,
                        Scope = re.coupon.Scope,
                        StartDate = re.coupon.StartDate,
                        EndDate = re.coupon.EndDate,
                        IsActive = re.coupon.IsActive,
                        AllowedCategoriesCsv = re.coupon.AllowedCategoriesCsv
                    };
                }
            }

            if (best == null)
            {
                SetAppliedCoupon(null);
                TempData["CartMessage"] = bestMessage;
            }
            else
            {
                SetAppliedCoupon(best); // áp 1 mã tốt nhất
                TempData["CartMessage"] = $"Đã áp dụng mã {best.Code}. {best.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // Gỡ mã – hỗ trợ AJAX hoặc post thường
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveCoupon(bool ajax = false)
        {
            SetAppliedCoupon(null);

            var cart = _cart.GetCart();
            var (subtotal, discount, total) = await RecalcTotalsWithCouponAsync(cart); // discount sẽ = 0
            TempData["CartMessage"] = "Đã bỏ mã giảm giá.";

            var isAjax = ajax || string.Equals(Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);
            if (isAjax)
            {
                return Json(new { ok = true, subtotal, discount, total });
            }
            return RedirectToAction(nameof(Index));
        }

        // ====== Helper: subtotal -> apply coupon -> clamp -> total ======
        private async Task<(decimal subtotal, decimal discount, decimal total)> RecalcTotalsWithCouponAsync(List<CartItem> cart)
        {
            var subtotal = cart.Sum(x => x.LineTotal);
            var applied = GetAppliedCoupon();
            decimal discount = 0m;

            if (applied != null)
            {
                var re = await _coupon.TryApplyAsync(applied.Code, cart);
                if (re.ok && re.coupon != null)
                {
                    applied.Discount = re.discount;
                    applied.DiscountType = re.coupon.DiscountType;
                    applied.DiscountValue = re.coupon.DiscountValue;
                    applied.MinOrderAmount = re.coupon.MinOrderAmount;
                    applied.Scope = re.coupon.Scope;
                    applied.StartDate = re.coupon.StartDate;
                    applied.EndDate = re.coupon.EndDate;
                    applied.IsActive = re.coupon.IsActive;
                    applied.AllowedCategoriesCsv = re.coupon.AllowedCategoriesCsv;

                    SetAppliedCoupon(applied);
                    discount = re.discount;
                }
                else
                {
                    SetAppliedCoupon(null); // không còn đủ điều kiện
                }
            }

            if (discount > subtotal) discount = subtotal;
            var total = subtotal - discount;
            return (subtotal, discount, total);
        }
    }
}
