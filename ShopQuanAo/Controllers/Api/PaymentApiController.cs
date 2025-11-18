using Microsoft.AspNetCore.Mvc;
using ShopQuanAo.Models;
using ShopQuanAo.Services;

namespace ShopQuanAo.Controllers
{
    /// <summary>
    /// API thanh toán (MoMo, VNPay) dùng cho Swagger / JS client.
    /// Không thay thế PaymentController hiện tại, chỉ là lớp API mỏng.
    /// </summary>
    [ApiController]
    [Route("api/payment")]
    [Produces("application/json")]
    public class PaymentApiController : ControllerBase
    {
        private readonly IMomoService _momoService;
        private readonly IVnPayService _vnPayService;

        public PaymentApiController(IMomoService momoService, IVnPayService vnPayService)
        {
            _momoService = momoService;
            _vnPayService = vnPayService;
        }

        /// <summary>
        /// Tạo link thanh toán MoMo cho đơn hàng.
        /// </summary>
        /// <remarks>
        /// Body mẫu:
        /// {
        ///   "fullName": "Nguyễn Văn A",
        ///   "amount": 250000,
        ///   "orderInfo": "Thanh toán đơn hàng tại ShopQuanAo"
        /// }
        /// </remarks>
        [HttpPost("momo")]
        public async Task<IActionResult> CreateMomoPayment([FromBody] OrderInfoModel model)
        {
            if (model == null || model.Amount <= 0)
                return BadRequest(new { message = "Dữ liệu không hợp lệ hoặc số tiền <= 0." });

            var res = await _momoService.CreatePaymentAsync(model);

            if (string.IsNullOrWhiteSpace(res.PayUrl))
            {
                return BadRequest(new
                {
                    ok = false,
                    message = $"MoMo tạo giao dịch thất bại: {res.Message}"
                });
            }

            return Ok(new
            {
                ok = true,
                gateway = "MoMo",
                payUrl = res.PayUrl,
                message = res.Message
            });
        }

        /// <summary>
        /// Tạo link thanh toán VNPay cho đơn hàng.
        /// </summary>
        /// <remarks>
        /// Body mẫu:
        /// {
        ///   "name": "Nguyễn Văn A",
        ///   "amount": 250000,
        ///   "orderDescription": "Đơn hàng ShopQuanAo",
        ///   "orderType": "other"
        /// }
        /// </remarks>
        [HttpPost("vnpay")]
        public IActionResult CreateVnpayPayment([FromBody] PaymentInformationModel model)
        {
            if (model == null || model.Amount <= 0)
                return BadRequest(new { message = "Dữ liệu không hợp lệ hoặc số tiền <= 0." });

            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);

            return Ok(new
            {
                ok = true,
                gateway = "VNPay",
                payUrl = url
            });
        }
    }
}
