using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ShopQuanAo.Data;
using ShopQuanAo.Models;
using ShopQuanAo.Services;
using ShopQuanAo.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace ShopQuanAo.Controllers
{
    public class CalcShipFeeRequest
    {
        public string? ProvinceName { get; set; }
        public string? DistrictName { get; set; }
        public string? WardName { get; set; }
        public string? AddressDetail { get; set; }
    }
    public class CheckoutController : Controller
    {
        private readonly ICartService _cartService;
        private readonly ICouponService _couponService;
        private readonly IVnPayService _vnPayService;
        private readonly IMomoService _momoService;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClientFactory;

        private const string COUPON_KEY = "CART_COUPON";

        public CheckoutController(
            ICartService cartService,
            ICouponService couponService,
            IVnPayService vnPayService,
            IMomoService momoService,
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager,
            IConfiguration config,
            IHttpClientFactory httpClientFactory
        )
        {
            _cartService = cartService;
            _couponService = couponService;
            _vnPayService = vnPayService;
            _momoService = momoService;
            _db = db;
            _userManager = userManager;
            _config = config;
            _httpClientFactory = httpClientFactory;
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
            public int ProvinceId { get; set; }
            public string ProvinceName { get; set; } = "";
            public int DistrictId { get; set; }
            public string DistrictName { get; set; } = "";
            public int WardId { get; set; }
            public string WardName { get; set; } = "";
            public string AddressDetail { get; set; } = "";
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

        // ======================= API: TỈNH / QUẬN / PHƯỜNG (TỪ GHN API) =======================

        [HttpGet]
        public async Task<IActionResult> GetProvinces()
        {
            // Có thể dùng GHN API hoặc API khác
            // Ví dụ: GHN API
            var baseUrl = _config["GHN:BaseUrl"] ?? "https://online-gateway.ghn.vn/shiip/public-api/v2";
            var token = _config["GHN:Token"] ?? "";

            if (string.IsNullOrWhiteSpace(token))
            {
                return Json(new { code = 400, message = "GHN Token chưa được cấu hình" });
            }

            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                httpClient.Timeout = TimeSpan.FromSeconds(30);
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("Token", token);
                
                var response = await httpClient.GetAsync($"{baseUrl}/master-data/province");
                var content = await response.Content.ReadAsStringAsync();
                
                if (response.IsSuccessStatusCode)
                {
                    // Trả về trực tiếp content từ GHN API
                    Response.ContentType = "application/json";
                    return Content(content, "application/json");
                }
                
                // Log lỗi để debug
                Console.WriteLine($"GHN API Error: Status={response.StatusCode}, Content={content}");
                return Json(new { code = (int)response.StatusCode, message = "Không thể lấy danh sách tỉnh/thành phố từ GHN API", data = new object[0] });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetProvinces: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return Json(new { code = 500, message = $"Lỗi: {ex.Message}", data = new object[0] });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDistricts(int provinceId)
        {
            var baseUrl = _config["GHN:BaseUrl"] ?? "https://online-gateway.ghn.vn/shiip/public-api/v2";
            var token = _config["GHN:Token"] ?? "";

            if (string.IsNullOrWhiteSpace(token))
            {
                return Json(new { code = 400, message = "GHN Token chưa được cấu hình", data = new object[0] });
            }

            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                httpClient.Timeout = TimeSpan.FromSeconds(30);
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("Token", token);
                
                var response = await httpClient.GetAsync($"{baseUrl}/master-data/district?province_id={provinceId}");
                var content = await response.Content.ReadAsStringAsync();
                
                if (response.IsSuccessStatusCode)
                {
                    Response.ContentType = "application/json";
                    return Content(content, "application/json");
                }
                
                Console.WriteLine($"GHN API Error: Status={response.StatusCode}, Content={content}");
                return Json(new { code = (int)response.StatusCode, message = "Không thể lấy danh sách quận/huyện", data = new object[0] });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetDistricts: {ex.Message}");
                return Json(new { code = 500, message = $"Lỗi: {ex.Message}", data = new object[0] });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetWards(int districtId)
        {
            var baseUrl = _config["GHN:BaseUrl"] ?? "https://online-gateway.ghn.vn/shiip/public-api/v2";
            var token = _config["GHN:Token"] ?? "";

            if (string.IsNullOrWhiteSpace(token))
            {
                return Json(new { code = 400, message = "GHN Token chưa được cấu hình", data = new object[0] });
            }

            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                httpClient.Timeout = TimeSpan.FromSeconds(30);
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("Token", token);
                
                var response = await httpClient.GetAsync($"{baseUrl}/master-data/ward?district_id={districtId}");
                var content = await response.Content.ReadAsStringAsync();
                
                if (response.IsSuccessStatusCode)
                {
                    Response.ContentType = "application/json";
                    return Content(content, "application/json");
                }
                
                Console.WriteLine($"GHN API Error: Status={response.StatusCode}, Content={content}");
                return Json(new { code = (int)response.StatusCode, message = "Không thể lấy danh sách phường/xã", data = new object[0] });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetWards: {ex.Message}");
                return Json(new { code = 500, message = $"Lỗi: {ex.Message}", data = new object[0] });
            }
        }

        // ======================= API TÍNH PHÍ SHIP (THỦ CÔNG) =======================

        [HttpPost]
        public async Task<IActionResult> CalcShipFee([FromBody] ShippingAddressVM addr)
        {
            if (string.IsNullOrWhiteSpace(addr.ProvinceName))
            {
                return Json(new
                {
                    ok = false,
                    shipFee = 0,
                    message = "Vui lòng chọn tỉnh/thành phố."
                });
            }

            // Tìm phí ship theo tên tỉnh/thành phố
            var shippingFee = await _db.ShippingFees
                .FirstOrDefaultAsync(sf => sf.ProvinceName.ToLower().Trim() == addr.ProvinceName.ToLower().Trim() && sf.IsActive);

            if (shippingFee == null)
            {
                return Json(new
                {
                    ok = false,
                    shipFee = 0,
                    message = "Chưa có cấu hình phí ship cho tỉnh/thành phố này. Vui lòng liên hệ admin."
                });
            }

            return Json(new { ok = true, shipFee = (int)shippingFee.Fee });
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

            // Địa chỉ hiển thị/lưu DB (lấy từ API địa chỉ)
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

    }
}
