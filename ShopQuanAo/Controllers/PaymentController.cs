using Microsoft.AspNetCore.Mvc;
using ShopQuanAo.Models;
using ShopQuanAo.Services;

namespace ShopQuanAo.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IMomoService _momoService;
        private readonly IVnPayService _vn;
        private readonly ICartService _cartService;

        public PaymentController(IMomoService momoService, IVnPayService vn, ICartService cartService)
        {
            _momoService = momoService;
            _vn = vn;
            _cartService = cartService;
        }

        // MoMo
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

        [HttpGet]
        [Route("Checkout/PaymentCallBack")]
        public IActionResult PaymentCallBack()
        {
            var result = _momoService.PaymentExecuteAsync(Request.Query);
            return Json(result);
        }

        // VNPay
        [HttpPost]
        public IActionResult CreatePaymentUrlVnpay(PaymentInformationModel m)
            => Redirect(_vn.CreatePaymentUrl(m, HttpContext));

        [HttpGet]
        public IActionResult PaymentCallbackVnpay()
        {
            var res = _vn.PaymentExecute(Request.Query);
            
            if (res.Success)
            {
                _cartService.SaveCart(new List<Models.CartItem>());
                TempData["Success"] = "Thanh toán VNPay thành công!";
                return RedirectToAction("Result", "Checkout", new { code = res.OrderId, status = "success" });
            }
            else
            {
                TempData["Error"] = "Thanh toán VNPay thất bại.";
                return RedirectToAction("Result", "Checkout", new { code = res.OrderId, status = "failed", message = res.VnPayResponseCode });
            }
        }
    }
}
