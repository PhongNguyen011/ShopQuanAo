using Microsoft.AspNetCore.Mvc;
using ShopQuanAo.Models;
using ShopQuanAo.Services;

namespace ShopQuanAo.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IVnPayService _vn;
        private readonly ICartService _cartService;

        public PaymentController(IVnPayService vn, ICartService cartService)
        {
            _vn = vn;
            _cartService = cartService;
        }

        [HttpPost]
        public IActionResult CreatePaymentUrlVnpay(PaymentInformationModel m)
            => Redirect(_vn.CreatePaymentUrl(m, HttpContext));

        [HttpGet]
        public IActionResult PaymentCallbackVnpay()
        {
            var res = _vn.PaymentExecute(Request.Query);
            
            if (res.Success)
            {
                // Clear cart after successful payment
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
