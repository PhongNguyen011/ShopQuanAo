using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ShopQuanAo.Data;
using ShopQuanAo.Models;
using ShopQuanAo.Services;
using ShopQuanAo.Utils;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Text;

namespace ShopQuanAo.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ICartService _cartService;
        private readonly ICouponService _couponService;
        private readonly IVnPayService _vnPayService;
        private readonly IMomoService _momoService;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGhnShippingService _ghnShippingService;
        private readonly IConfiguration _config;

        private const string COUPON_KEY = "CART_COUPON";

        public CheckoutController(
            ICartService cartService,
            ICouponService couponService,
            IVnPayService vnPayService,
            IMomoService momoService,
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager,
            IGhnShippingService ghnShippingService,
            IConfiguration config
        )
        {
            _cartService = cartService;
            _couponService = couponService;
            _vnPayService = vnPayService;
            _momoService = momoService;
            _db = db;
            _userManager = userManager;
            _ghnShippingService = ghnShippingService;
            _config = config;
        }

        // ======================= VIEW MODELS / DTOs =======================

        // Dùng để lưu tạm thông tin mã giảm giá trong Session
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

        // Request body cho CalcShipFee
        public class ShippingAddressVM
        {
            public int ToDistrictId { get; set; }        // GHN DistrictID
            public string ToWardCode { get; set; } = ""; // GHN WardCode
            public int? Weight { get; set; }             // gram
            public int? Length { get; set; }             // cm
            public int? Width { get; set; }              // cm
            public int? Height { get; set; }             // cm
            public int? InsuranceValue { get; set; }     // VNĐ
        }

        // ======================= HELPERS =======================

        private AppliedCouponInfo? GetAppliedCoupon()
            => HttpContext.Session.GetObject<AppliedCouponInfo>(COUPON_KEY);

        private async Task<(decimal subtotal, decimal discount, decimal total)> CalcTotalsAsync(List<CartItem> cart)
        {
            var subtotal = cart.Sum(x => x.LineTotal); // tổng tiền hàng
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

                    // không bao giờ giảm quá subtotal
                    discount = Math.Min(re.discount, subtotal);
                }
            }

            var total = subtotal - discount;
            return (subtotal, discount, total);
        }

        // ======================= PAGE /CHECKOUT =======================

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

        // ======================= COUPON =======================

        [HttpPost]
        [ValidateAntiForgeryToken]
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

            var tryCodes = code.Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                               .Select(s => s.Trim().ToUpperInvariant())
                               .Distinct()
                               .ToList();

            AppliedCouponInfo? bestCoupon = null;
            decimal bestDiscount = 0m;
            string bestMessage = "Không có mã hợp lệ.";

            foreach (var c in tryCodes)
            {
                var re = await _couponService.TryApplyAsync(c, cart);
                if (re.ok && re.coupon != null && re.discount > bestDiscount)
                {
                    bestDiscount = re.discount;
                    bestMessage = re.message;

                    bestCoupon = new AppliedCouponInfo
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

            if (bestCoupon == null)
            {
                HttpContext.Session.Remove(COUPON_KEY);
                TempData["CartMessage"] = bestMessage;
            }
            else
            {
                HttpContext.Session.SetObject(COUPON_KEY, bestCoupon);
                TempData["CartMessage"] = $"Đã áp dụng mã {bestCoupon.Code}. {bestCoupon.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveCoupon()
        {
            HttpContext.Session.Remove(COUPON_KEY);
            TempData["CartMessage"] = "Đã bỏ mã giảm giá.";
            return RedirectToAction(nameof(Index));
        }

        // ======================= API GHN: TỈNH / QUẬN / PHƯỜNG =======================
        // Frontend gọi để nạp dropdown

        [HttpGet]
        public async Task<IActionResult> GetProvincesGhn()
        {
            var baseUrl = _config["GHN:BaseUrl"];
            var token = _config["GHN:Token"];

            using var http = new HttpClient();
            http.DefaultRequestHeaders.Add("Token", token);

            var resp = await http.GetAsync($"{baseUrl}/master-data/province");
            var text = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
            {
                return Json(new { ok = false, message = "GHN province API error", raw = text });
            }

            // Trả y nguyên JSON GHN { code, data:[...] } để FE dùng trực tiếp
            return Content(text, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> GetDistrictsGhn(int provinceId)
        {
            var baseUrl = _config["GHN:BaseUrl"];
            var token = _config["GHN:Token"];

            using var http = new HttpClient();
            http.DefaultRequestHeaders.Add("Token", token);

            var resp = await http.GetAsync($"{baseUrl}/master-data/district?province_id={provinceId}");
            var text = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
            {
                return Json(new { ok = false, message = "GHN district API error", raw = text });
            }

            return Content(text, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> GetWardsGhn(int districtId)
        {
            var baseUrl = _config["GHN:BaseUrl"];
            var token = _config["GHN:Token"];

            using var http = new HttpClient();
            http.DefaultRequestHeaders.Add("Token", token);

            var resp = await http.GetAsync($"{baseUrl}/master-data/ward?district_id={districtId}");
            var text = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
            {
                return Json(new { ok = false, message = "GHN ward API error", raw = text });
            }

            return Content(text, "application/json");
        }

        // ======================= API TÍNH PHÍ SHIP GHN =======================

        [HttpPost]
        public async Task<IActionResult> CalcShipFee([FromBody] ShippingAddressVM addr)
        {
            var fee = await _ghnShippingService.CalculateShippingFeeAsync(
                addr.ToDistrictId,
                addr.ToWardCode,
                addr.Weight ?? 500,
                addr.Length ?? 20,
                addr.Width ?? 20,
                addr.Height ?? 10,
                addr.InsuranceValue ?? 200000
            );

            if (fee == null)
            {
                return Json(new
                {
                    ok = false,
                    shipFee = 0,
                    message = "GHN không trả về phí ship. Có thể khu vực chưa được hỗ trợ."
                });
            }

            return Json(new { ok = true, shipFee = fee });
        }

        // ======================= THANH TOÁN / ĐẶT HÀNG =======================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pay(
            string PaymentMethod,
            string CustomerName,
            string Phone,
            string ProvinceName,
            string DistrictName,
            string WardName,
            string AddressDetail,
            int ShippingFee,
            string? Note
        )
        {
            var cart = _cartService.GetCart();
            if (cart.Count == 0)
            {
                TempData["CartMessage"] = "Giỏ hàng trống.";
                return RedirectToAction("Index", "Cart");
            }

            var (subtotal, discount, total) = await CalcTotalsAsync(cart);

            // Địa chỉ hiển thị/lưu DB
            string fullAddress = $"{AddressDetail}, {WardName}, {DistrictName}, {ProvinceName}";

            // Tổng cuối = total + phí ship nếu chưa free ship
            decimal grandTotal = total;
            if (total < 500_000 && ShippingFee > 0)
            {
                grandTotal += ShippingFee;
            }

            var method = (PaymentMethod ?? "").Trim().ToUpperInvariant();

            // ----- VNPay -----
            if (method == "VNPAY")
            {
                var paymentInfo = new PaymentInformationModel
                {
                    Name = CustomerName ?? "Khách hàng",
                    Amount = (double)grandTotal,
                    OrderDescription = $"Đơn hàng ShopQuanAo",
                    OrderType = "other"
                };

                var payUrl = _vnPayService.CreatePaymentUrl(paymentInfo, HttpContext);
                return Redirect(payUrl);
            }

            // ----- MoMo -----
            if (method == "MOMO")
            {
                var orderInfo = new OrderInfoModel
                {
                    FullName = CustomerName ?? "Khách hàng",
                    Amount = grandTotal,
                    OrderInfo = "Thanh toán đơn hàng qua MoMo tại ShopQuanAo"
                };

                var res = await _momoService.CreatePaymentAsync(orderInfo);
                if (!string.IsNullOrWhiteSpace(res.PayUrl))
                    return Redirect(res.PayUrl);

                TempData["Error"] = $"MoMo tạo giao dịch thất bại: {res.Message}";
                return RedirectToAction(nameof(Result), new
                {
                    code = orderInfo.OrderId,
                    status = "failed",
                    message = res.Message
                });
            }

            // ----- COD -----
            if (method == "COD")
            {
                var userId = User.Identity.IsAuthenticated ? _userManager.GetUserId(User) : null;

                var order = new Order
                {
                    OrderCode = "OD" + DateTime.Now.Ticks,
                    CustomerName = CustomerName ?? "",
                    Phone = Phone ?? "",
                    Address = fullAddress,
                    PaymentMethod = "COD",
                    Subtotal = subtotal,
                    Discount = discount,
                    ShippingFee = ShippingFee,
                    Total = grandTotal,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow,
                    ApplicationUserId = userId,
                    Note = Note,
                    OrderItems = cart.Select(x => new OrderItem
                    {
                        ProductName = x.Name,
                        ProductImage = x.Image,
                        Price = x.Price,
                        Quantity = x.Quantity,
                        LineTotal = x.Price * x.Quantity
                    }).ToList()
                };

                _db.Orders.Add(order);
                _db.SaveChanges();

                _cartService.SaveCart(new List<CartItem>());

                TempData["Success"] = $"Đặt hàng COD thành công! Tổng thanh toán: {grandTotal:N0}đ (bao gồm phí ship: {ShippingFee:N0}đ)";
                return RedirectToAction(nameof(Result), new
                {
                    code = order.OrderCode,
                    status = "success"
                });
            }

            // ----- Unknown -----
            TempData["Error"] = "Phương thức thanh toán không hợp lệ.";
            return RedirectToAction(nameof(Result), new
            {
                code = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(),
                status = "failed",
                message = "INVALID_PAYMENT_METHOD"
            });
        }

        // ======================= RETURN TỪ VNPAY =======================

        [HttpGet]
        public IActionResult VnPayReturn()
        {
            // Ghi log full query từ VNPay để debug chữ ký
            Console.WriteLine("VNPay Return Params: " +
                string.Join(", ", HttpContext.Request.Query.Select(x => $"{x.Key}={x.Value}")));

            var result = _vnPayService.PaymentExecute(HttpContext.Request.Query);

            if (result.Success)
            {
                _cartService.SaveCart(new List<CartItem>());
                TempData["Success"] = "Thanh toán VNPay thành công!";
                return RedirectToAction(nameof(Result), new
                {
                    code = result.OrderId,
                    status = "success"
                });
            }
            else
            {
                TempData["Error"] = $"Thanh toán VNPay thất bại. Mã lỗi: {result.VnPayResponseCode}";
                return RedirectToAction(nameof(Result), new
                {
                    code = result.OrderId,
                    status = "failed",
                    message = result.VnPayResponseCode
                });
            }
        }

        // ======================= TRANG KẾT QUẢ CHUNG =======================

        [HttpGet]
        public IActionResult Result(string code, string status, string? message = null)
        {
            ViewBag.Code = code;
            ViewBag.Status = status;
            ViewBag.Message = message;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GhnProvinces()
        {
            var token = _config["GHN:Token"] ?? string.Empty;
            var baseUrl = _config["GHN:BaseUrl"] ?? "https://dev-online-gateway.ghn.vn/shiip/public-api";
            using var http = new HttpClient();
            http.DefaultRequestHeaders.Add("Token", token);
            var res = await http.GetAsync($"{baseUrl.TrimEnd('/')}/master-data/province");
            var text = await res.Content.ReadAsStringAsync();
            return Content(text, "application/json", Encoding.UTF8);
        }

        [HttpGet]
        public async Task<IActionResult> GhnDistricts(int provinceId)
        {
            var token = _config["GHN:Token"] ?? string.Empty;
            var baseUrl = _config["GHN:BaseUrl"] ?? "https://dev-online-gateway.ghn.vn/shiip/public-api";
            using var http = new HttpClient();
            http.DefaultRequestHeaders.Add("Token", token);
            var url = $"{baseUrl.TrimEnd('/')}/master-data/district?province_id={provinceId}";
            var res = await http.GetAsync(url);
            var text = await res.Content.ReadAsStringAsync();
            return Content(text, "application/json", Encoding.UTF8);
        }

        [HttpGet]
        public async Task<IActionResult> GhnWards(int districtId)
        {
            var token = _config["GHN:Token"] ?? string.Empty;
            var baseUrl = _config["GHN:BaseUrl"] ?? "https://dev-online-gateway.ghn.vn/shiip/public-api";
            using var http = new HttpClient();
            http.DefaultRequestHeaders.Add("Token", token);
            var url = $"{baseUrl.TrimEnd('/')}/master-data/ward?district_id={districtId}";
            var res = await http.GetAsync(url);
            var text = await res.Content.ReadAsStringAsync();
            return Content(text, "application/json", Encoding.UTF8);
        }
    }
}
