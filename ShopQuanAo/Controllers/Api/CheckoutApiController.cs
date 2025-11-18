using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopQuanAo.Data;
using ShopQuanAo.Models.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using static ShopQuanAo.Controllers.CheckoutController;

namespace ShopQuanAo.Controllers.Api
{
    [ApiController]
    [Route("api/checkout")]
    [Produces("application/json")]
    public class CheckoutApiController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApplicationDbContext _db;

        public CheckoutApiController(
            IConfiguration config,
            IHttpClientFactory httpClientFactory,
            ApplicationDbContext db)
        {
            _config = config;
            _httpClientFactory = httpClientFactory;
            _db = db;
        }

        // ======================= GHN PROVINCES =======================
        /// <summary>Lấy danh sách tỉnh/thành từ GHN.</summary>
        [HttpGet("provinces")]
        public async Task<IActionResult> GetProvinces()
        {
            var baseUrl = _config["GHN:BaseUrl"] ?? "https://online-gateway.ghn.vn/shiip/public-api/v2";
            var token = _config["GHN:Token"] ?? "";

            if (string.IsNullOrWhiteSpace(token))
            {
                return BadRequest(new { code = 400, message = "GHN Token chưa được cấu hình" });
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
                    // trả nguyên JSON từ GHN
                    return Content(content, "application/json");
                }

                Console.WriteLine($"GHN API Error: Status={response.StatusCode}, Content={content}");
                return StatusCode((int)response.StatusCode, new
                {
                    code = (int)response.StatusCode,
                    message = "Không thể lấy danh sách tỉnh/thành phố từ GHN API",
                    data = Array.Empty<object>()
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetProvinces: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return StatusCode(500, new { code = 500, message = $"Lỗi: {ex.Message}", data = Array.Empty<object>() });
            }
        }

        // ======================= GHN DISTRICTS =======================
        /// <summary>Lấy danh sách quận/huyện theo provinceId từ GHN.</summary>
        [HttpGet("districts")]
        public async Task<IActionResult> GetDistricts([FromQuery] int provinceId)
        {
            var baseUrl = _config["GHN:BaseUrl"] ?? "https://online-gateway.ghn.vn/shiip/public-api/v2";
            var token = _config["GHN:Token"] ?? "";

            if (string.IsNullOrWhiteSpace(token))
            {
                return BadRequest(new { code = 400, message = "GHN Token chưa được cấu hình", data = Array.Empty<object>() });
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
                    return Content(content, "application/json");
                }

                Console.WriteLine($"GHN API Error: Status={response.StatusCode}, Content={content}");
                return StatusCode((int)response.StatusCode, new
                {
                    code = (int)response.StatusCode,
                    message = "Không thể lấy danh sách quận/huyện",
                    data = Array.Empty<object>()
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetDistricts: {ex.Message}");
                return StatusCode(500, new { code = 500, message = $"Lỗi: {ex.Message}", data = Array.Empty<object>() });
            }
        }

        // ======================= GHN WARDS =======================
        /// <summary>Lấy danh sách phường/xã theo districtId từ GHN.</summary>
        [HttpGet("wards")]
        public async Task<IActionResult> GetWards([FromQuery] int districtId)
        {
            var baseUrl = _config["GHN:BaseUrl"] ?? "https://online-gateway.ghn.vn/shiip/public-api/v2";
            var token = _config["GHN:Token"] ?? "";

            if (string.IsNullOrWhiteSpace(token))
            {
                return BadRequest(new { code = 400, message = "GHN Token chưa được cấu hình", data = Array.Empty<object>() });
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
                    return Content(content, "application/json");
                }

                Console.WriteLine($"GHN API Error: Status={response.StatusCode}, Content={content}");
                return StatusCode((int)response.StatusCode, new
                {
                    code = (int)response.StatusCode,
                    message = "Không thể lấy danh sách phường/xã",
                    data = Array.Empty<object>()
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetWards: {ex.Message}");
                return StatusCode(500, new { code = 500, message = $"Lỗi: {ex.Message}", data = Array.Empty<object>() });
            }
        }

        // ======================= CALC SHIP FEE (DB SHIPPING_FEES) =======================
        /// <summary>Tính phí ship theo tên tỉnh/thành đã cấu hình trong bảng ShippingFees.</summary>
        [HttpPost("calc-ship-fee")]
        public async Task<IActionResult> CalcShipFee([FromBody] ShippingAddressVM addr)
        {
            if (addr == null || string.IsNullOrWhiteSpace(addr.ProvinceName))
            {
                return Ok(new
                {
                    ok = false,
                    shipFee = 0,
                    message = "Vui lòng chọn tỉnh/thành phố."
                });
            }

            var provinceNameNorm = addr.ProvinceName.Trim().ToLower();

            var shippingFee = await _db.ShippingFees
                .Where(sf => sf.IsActive)
                .FirstOrDefaultAsync(sf => sf.ProvinceName.ToLower().Trim() == provinceNameNorm);

            if (shippingFee == null)
            {
                return Ok(new
                {
                    ok = false,
                    shipFee = 0,
                    message = "Chưa có cấu hình phí ship cho tỉnh/thành phố này. Vui lòng liên hệ admin."
                });
            }

            return Ok(new
            {
                ok = true,
                shipFee = (int)shippingFee.Fee,
                message = $"Phí ship áp dụng cho {shippingFee.ProvinceName} là {shippingFee.Fee:N0}đ."
            });
        }
    }
}
