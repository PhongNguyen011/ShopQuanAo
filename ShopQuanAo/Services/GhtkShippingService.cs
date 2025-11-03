using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace ShopQuanAo.Services
{
    public class GhtkShippingService : IGhtkShippingService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public GhtkShippingService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<int?> CalculateShippingFeeAsync(
            string toProvinceName,
            string toDistrictName,
            string toWardName,
            string toAddressDetail,
            int weightGram,
            int insuranceValueVnd)
        {
            var baseUrl = (_configuration["GHTK:BaseUrl"] ?? "https://services.giaohangtietkiem.vn").TrimEnd('/');
            var token = _configuration["GHTK:Token"] ?? string.Empty;
            var pickProvince = _configuration["GHTK:PickProvince"] ?? string.Empty;
            var pickDistrict = _configuration["GHTK:PickDistrict"] ?? string.Empty;
            var pickAddress = _configuration["GHTK:PickAddress"] ?? string.Empty;

            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(pickProvince) || string.IsNullOrWhiteSpace(pickDistrict))
            {
                return null;
            }

            _httpClient.DefaultRequestHeaders.Remove("Token");
            _httpClient.DefaultRequestHeaders.Add("Token", token);

            var toAddress = string.Join(
                ", ",
                new[] { toAddressDetail, toWardName, toDistrictName, toProvinceName }
                    .Where(s => !string.IsNullOrWhiteSpace(s))
            );

            var query = new Dictionary<string, string>
            {
                { "pick_province", pickProvince },
                { "pick_district", pickDistrict },
                { "pick_address", pickAddress },
                { "province", toProvinceName },
                { "district", toDistrictName },
                { "address", toAddress },
                { "weight", Math.Max(weightGram, 1).ToString() },
                { "value", Math.Max(insuranceValueVnd, 0).ToString() },
                { "transport", "road" }
            };

            var url = baseUrl + "/services/shipment/fee?" + string.Join("&", query.Select(kv => Uri.EscapeDataString(kv.Key) + "=" + Uri.EscapeDataString(kv.Value)));

            var resp = await _httpClient.GetAsync(url);
            var text = await resp.Content.ReadAsStringAsync();
            if (!resp.IsSuccessStatusCode)
            {
                return null;
            }

            try
            {
                using var doc = JsonDocument.Parse(text);
                var root = doc.RootElement;
                if (root.TryGetProperty("success", out var okProp) && okProp.ValueKind == JsonValueKind.True)
                {
                    if (root.TryGetProperty("fee", out var feeObj))
                    {
                        if (feeObj.TryGetProperty("fee", out var feeValue) && feeValue.ValueKind == JsonValueKind.Number)
                        {
                            return feeValue.GetInt32();
                        }
                    }
                }
            }
            catch
            {
                return null;
            }

            return null;
        }
    }
}


