using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopQuanAo.Models;
using ShopQuanAo.Services;

namespace ShopQuanAo.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IMomoService _momoService;
        private readonly IVnPayService _vnPayService;
        private readonly ICartService _cartService;

        public PaymentController(IMomoService momoService, IVnPayService vnPayService, ICartService cartService)
        {
            _momoService = momoService;
            _vnPayService = vnPayService;
            _cartService = cartService;
        }

        // ===================== MOMO PAYMENT =====================
        [HttpPost]
        [Route("CreatePaymentUrl")]
        public async Task<IActionResult> CreatePaymentUrl(OrderInfoModel model)
        {
            var res = await _momoService.CreatePaymentAsync(model);
            if (!string.IsNullOrWhiteSpace(res.PayUrl))
                return Redirect(res.PayUrl);

            TempData["Error"] = $"MoMo tạo giao dịch thất bại: {res.Message}";
            return RedirectToAction("Result", "Checkout", new { code = model.OrderId, status = "failed", message = res.Message });
        }

        // Callback từ MoMo (ReturnUrl)
        [AcceptVerbs("GET", "POST")]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        [Route("Checkout/PaymentCallBack")]
        public IActionResult PaymentCallBack()
        {
            try
            {
                string Get(string key)
                    => (Request.HasFormContentType && Request.Form.ContainsKey(key))
                        ? Request.Form[key].ToString()
                        : Request.Query[key].ToString();

                string GetAny(params string[] keys)
                {
                    foreach (var k in keys)
                    {
                        var v = Get(k);
                        if (!string.IsNullOrWhiteSpace(v)) return v;
                    }
                    return string.Empty;
                }

                // MoMo có nơi trả resultCode, có nơi trả errorCode = "0" khi thành công
                var resultCode = GetAny("resultCode", "ResultCode", "errorCode", "ErrorCode");
                var orderId = GetAny("orderId", "orderID", "OrderId", "OrderID");
                var message = GetAny("message", "Message");

                Console.WriteLine($"[MoMo.Callback] resultCode={resultCode}, orderId={orderId}, message={message}");

                if (resultCode == "0")
                {
                    // Thanh toán thành công
                    _cartService.SaveCart(new List<Models.CartItem>());
                    TempData["Success"] = "Thanh toán MoMo thành công!";
                    return RedirectToAction("Result", "Checkout",
                        new { code = orderId, status = "success", message = "Thanh toán MoMo thành công!" });
                }

                // Thanh toán thất bại hoặc bị hủy
                TempData["Error"] = string.IsNullOrWhiteSpace(message) ? "Thanh toán MoMo thất bại." : message;
                return RedirectToAction("Result", "Checkout",
                    new { code = orderId, status = "failed", message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MoMo.Callback][EX] {ex.Message}");
                TempData["Error"] = "Xử lý callback MoMo gặp lỗi.";
                return RedirectToAction("Result", "Checkout",
                    new { code = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(), status = "failed", message = "MOMO_CALLBACK_ERROR" });
            }
        }

        // ===================== VNPAY PAYMENT =====================
        [HttpPost]
        public IActionResult CreatePaymentUrlVnpay(PaymentInformationModel model)
            => Redirect(_vnPayService.CreatePaymentUrl(model, HttpContext));

        [HttpGet]
        public IActionResult PaymentCallbackVnpay()
        {
            var res = _vnPayService.PaymentExecute(Request.Query);
            if (res.Success)
            {
                _cartService.SaveCart(new List<Models.CartItem>());
                TempData["Success"] = "Thanh toán VNPay thành công!";
                return RedirectToAction("Result", "Checkout",
                    new { code = res.OrderId, status = "success" });
            }

            TempData["Error"] = "Thanh toán VNPay thất bại.";
            return RedirectToAction("Result", "Checkout",
                new { code = res.OrderId, status = "failed", message = res.VnPayResponseCode });
        }

        // IPN từ MoMo (server-to-server). Trả OK để tránh MoMo retry.
        [HttpPost]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        [Route("Checkout/MomoNotify")]
        public IActionResult MomoNotify()
        {
            Console.WriteLine("[MoMo.IPN] Received IPN");
            return Ok(new { result = "ok" });
        }
    }
}
