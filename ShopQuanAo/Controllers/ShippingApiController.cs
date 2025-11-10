using Microsoft.AspNetCore.Mvc;
using ShopQuanAo.Services;
using System.Threading.Tasks;

namespace ShopQuanAo.Controllers
{
    // Model nhận JSON từ JS bên Checkout/Index.cshtml
    public class ShippingCalcRequest
    {
        public string? ProvinceName { get; set; }
        public string? DistrictName { get; set; }
        public string? WardName { get; set; }
        public string? AddressDetail { get; set; }
    }

    /// <summary>
    /// API chuyên dùng cho tính phí ship.
    /// Route gốc: /Checkout/[action]
    /// </summary>
    [Route("Checkout/[action]")]
    public class ShippingApiController : Controller
    {
        private readonly IShippingFeeService _shippingFeeService;

        public ShippingApiController(IShippingFeeService shippingFeeService)
        {
            _shippingFeeService = shippingFeeService;
        }

        /// <summary>
        /// POST /Checkout/CalcShipFee
        /// Body JSON: { provinceName, districtName, wardName, addressDetail }
        /// Trả về: { ok, shipFee, message }
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CalcShipFee([FromBody] ShippingCalcRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.ProvinceName))
            {
                return Json(new
                {
                    ok = false,
                    shipFee = 0,
                    message = "Vui lòng chọn Tỉnh/Thành phố."
                });
            }

            // Tính phí ship theo Tỉnh/Thành (dùng bảng ShippingFees của bạn)
            var fee = await _shippingFeeService.GetFeeForProvinceAsync(req.ProvinceName);

            if (fee <= 0)
            {
                return Json(new
                {
                    ok = false,
                    shipFee = 0,
                    message = "Chưa cấu hình phí ship cho tỉnh/thành này. Vui lòng liên hệ shop."
                });
            }

            return Json(new
            {
                ok = true,
                shipFee = fee,
                message = ""
            });
        }
    }
}
