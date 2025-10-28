using Microsoft.AspNetCore.Mvc;
using ShopQuanAo.Models;
using ShopQuanAo.Services;
using ShopQuanAo.Utils;

namespace ShopQuanAo.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ICartService _cartService;
        private readonly ICouponService _couponService;
        private readonly IVnPayService _vnPayService;

        private const string COUPON_KEY = "CART_COUPON";

        public CheckoutController(ICartService cartService, ICouponService couponService, IVnPayService vnPayService)
        {
            _cartService = cartService;
            _couponService = couponService;
            _vnPayService = vnPayService;
        }

        // ===================== Models =====================
        public class AppliedCouponInfo
        {
            public string Code { get; set; } = string.Empty;
            public decimal Discount { get; set; }
            public string Message { get; set; } = "";
            public DiscountType DiscountType { get; set; }
            public decimal DiscountValue { get; set; }
            public decimal? MinOrderAmount { get; set; }
            public CouponScope Scope { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public bool IsActive { get; set; }
            public string? AllowedCategoriesCsv { get; set; }
        }

        public class CheckoutRequest
        {
            public string PaymentMethod { get; set; } = "COD"; // COD | VNPAY | MOMO
            public string? CustomerName { get; set; }
            public string? Phone { get; set; }
            public string? Address { get; set; }
            public string? Note { get; set; }
        }

        // ===================== Helpers =====================
        private AppliedCouponInfo? GetAppliedCoupon()
            => HttpContext.Session.GetObject<AppliedCouponInfo>(COUPON_KEY);

        private async Task<(decimal subtotal, decimal discount, decimal total)> CalcTotalsAsync(List<CartItem> cart)
        {
            var subtotal = cart.Sum(x => x.LineTotal);
            decimal discount = 0m;

            var applied = GetAppliedCoupon();
            if (applied != null && !string.IsNullOrWhiteSpace(applied.Code))
            {
                var re = await _couponService.TryApplyAsync(applied.Code, cart);
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
                    applied.Message = re.message;
                    HttpContext.Session.SetObject(COUPON_KEY, applied);

                    discount = Math.Min(re.discount, subtotal);
                }
            }

            var total = subtotal - discount;
            return (subtotal, discount, total);
        }

        // ===================== Pages =====================
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var cart = _cartService.GetCart();
            if (cart.Count == 0)
            {
                TempData["CartMessage"] = "Giỏ hàng trống.";
                return RedirectToAction("Index", "Cart");
            }

            var (subtotal, discount, total) = await CalcTotalsAsync(cart);
            ViewBag.Subtotal = subtotal;
            ViewBag.Discount = discount;
            ViewBag.Total = total;
            ViewBag.AppliedCoupon = GetAppliedCoupon();
            return View(cart);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ApplyCoupon(string code)
        {
            var cart = _cartService.GetCart();
            if (cart.Count == 0)
            {
                TempData["CartMessage"] = "Giỏ hàng trống.";
                return RedirectToAction(nameof(Index));
            }

            if (string.IsNullOrWhiteSpace(code))
            {
                HttpContext.Session.Remove(COUPON_KEY);
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
                var re = await _couponService.TryApplyAsync(c, cart);
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
                HttpContext.Session.Remove(COUPON_KEY);
                TempData["CartMessage"] = bestMessage;
            }
            else
            {
                HttpContext.Session.SetObject(COUPON_KEY, best);
                TempData["CartMessage"] = $"Đã áp dụng mã {best.Code}. {best.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult RemoveCoupon()
        {
            HttpContext.Session.Remove(COUPON_KEY);
            TempData["CartMessage"] = "Đã bỏ mã giảm giá.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Pay(CheckoutRequest req)
        {
            var cart = _cartService.GetCart();
            if (cart.Count == 0)
            {
                TempData["CartMessage"] = "Giỏ hàng trống.";
                return RedirectToAction("Index", "Cart");
            }

            var (_, _, total) = await CalcTotalsAsync(cart);

            switch ((req.PaymentMethod ?? "COD").ToUpperInvariant())
            {
                case "VNPAY":
                    {
                        var paymentInfo = new PaymentInformationModel
                        {
                            Name = req.CustomerName ?? "Khách hàng",
                            Amount = (double)total,
                            OrderDescription = $"Đơn hàng ShopQuanAo",
                            OrderType = "other"
                        };
                        var url = _vnPayService.CreatePaymentUrl(paymentInfo, HttpContext);
                        return Redirect(url);
                    }

                default: // COD
                    var orderId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
                    _cartService.SaveCart(new List<CartItem>());
                    TempData["Success"] = "Đặt hàng COD thành công! Chúng tôi sẽ liên hệ xác nhận.";
                    return RedirectToAction(nameof(Result), new { code = orderId, status = "success" });
            }
        }

        [HttpGet]
        public IActionResult VnPayReturn()
        {
            // Log tất cả query parameters để debug
            var queryParams = HttpContext.Request.Query;
            var logMessage = $"VNPay Return - Query params: {string.Join(", ", queryParams.Select(x => $"{x.Key}={x.Value}"))}";
            
            // Log vào console hoặc file log
            Console.WriteLine(logMessage);
            
            var result = _vnPayService.PaymentExecute(HttpContext.Request.Query);
            
            // Log kết quả xử lý
            Console.WriteLine($"VNPay Return - Success: {result.Success}, OrderId: {result.OrderId}, ResponseCode: {result.VnPayResponseCode}");
            
            if (result.Success)
            {
                _cartService.SaveCart(new List<CartItem>());
                TempData["Success"] = "Thanh toán VNPay thành công!";
                return RedirectToAction(nameof(Result), new { code = result.OrderId, status = "success" });
            }
            else
            {
                TempData["Error"] = $"Thanh toán VNPay thất bại. Mã lỗi: {result.VnPayResponseCode}";
                return RedirectToAction(nameof(Result), new { code = result.OrderId, status = "failed", message = result.VnPayResponseCode });
            }
        }

        [HttpGet]
        public IActionResult Result(string code, string status, string? message = null)
        {
            ViewBag.Code = code;
            ViewBag.Status = status;
            ViewBag.Message = message;
            return View();
        }
    }
}