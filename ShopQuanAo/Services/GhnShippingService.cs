using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace ShopQuanAo.Services
{
    public class GhnShippingService : IGhnShippingService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;

        public GhnShippingService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _config = config;
        }

        public async Task<int?> CalculateShippingFeeAsync(int toDistrictId, string toWardCode, int weight, int length, int width, int height, int insuranceValue)
        {
            var fromDistrictId = int.Parse(_config["GHN:FromDistrictId"] ?? "0");
            var fromWardCode = _config["GHN:FromWardCode"] ?? string.Empty;
            var shopId = int.Parse(_config["GHN:ShopId"] ?? "0");
            var baseUrl = (_config["GHN:BaseUrl"] ?? "https://dev-online-gateway.ghn.vn/shiip/public-api").TrimEnd('/');
            // Bước 1: Lấy serviceId phù hợp
            var availableServicesBody = new
            {
                shop_id = shopId,
                from_district = fromDistrictId,
                to_district = toDistrictId
            };
            var servicesContent = new StringContent(JsonSerializer.Serialize(availableServicesBody), Encoding.UTF8, "application/json");
            var servicesResp = await _http.PostAsync($"{baseUrl}/v2/shipping-order/available-services", servicesContent);
            var servicesText = await servicesResp.Content.ReadAsStringAsync();
            if (!servicesResp.IsSuccessStatusCode) return null;
            try
            {
                using var servicesDoc = JsonDocument.Parse(servicesText);
                if (!servicesDoc.RootElement.TryGetProperty("data", out var dataArr) || dataArr.GetArrayLength() == 0)
                    return null;
                var serviceId = dataArr[0].GetProperty("service_id").GetInt32();
                // Bước 2: Tính phí GHN với serviceId đã lấy được
                var feeBody = new
                {
                    from_district_id = fromDistrictId,
                    from_ward_code = fromWardCode,
                    to_district_id = toDistrictId,
                    to_ward_code = toWardCode,
                    service_id = serviceId,
                    shop_id = shopId,
                    weight = weight > 0 ? weight : 1200,
                    insurance_value = insuranceValue > 0 ? insuranceValue : 0
                };
                var feeContent = new StringContent(JsonSerializer.Serialize(feeBody), Encoding.UTF8, "application/json");
                var feeResp = await _http.PostAsync($"{baseUrl}/v2/shipping-order/fee", feeContent);
                var feeText = await feeResp.Content.ReadAsStringAsync();
                if (!feeResp.IsSuccessStatusCode) return null;
                using var feeDoc = JsonDocument.Parse(feeText);
                if (!feeDoc.RootElement.TryGetProperty("data", out var feeData) || !feeData.TryGetProperty("total", out var totalProp))
                    return null;
                return totalProp.GetInt32();
            }
            catch
            {
                return null;
            }
        }
    }
}
