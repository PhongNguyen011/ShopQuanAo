using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using ShopQuanAo.Models;

namespace ShopQuanAo.Services
{
    public class MomoService : IMomoService
    {
        private readonly IOptions<MomoOptionModel> _options;

        public MomoService(IOptions<MomoOptionModel> options)
        {
            _options = options;
        }

        private static string HmacSha256(string key, string data)
        {
            using var h = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            var bytes = h.ComputeHash(Encoding.UTF8.GetBytes(data));
            return BitConverter.ToString(bytes).Replace("-", string.Empty).ToLowerInvariant();
        }

        public async Task<MomoCreatePaymentResponseModel> CreatePaymentAsync(OrderInfoModel model)
        {
            // Chuẩn hóa dữ liệu đầu vào
            model.OrderId = DateTime.UtcNow.Ticks.ToString();
            model.OrderInfo = $"Khách hàng: {model.FullName}. Nội dung: {model.OrderInfo}";

            var endpoint = _options.Value.MomoApiUrl;
            var partnerCode = _options.Value.PartnerCode;
            var accessKey = _options.Value.AccessKey;
            var secretKey = _options.Value.SecretKey;
            var returnUrl = _options.Value.ReturnUrl;
            var notifyUrl = _options.Value.NotifyUrl;
            var requestType = _options.Value.RequestType;

            var amount = ((long)Math.Round(model.Amount)).ToString();
            var rawData = $"partnerCode={partnerCode}&accessKey={accessKey}&requestId={model.OrderId}&amount={amount}&orderId={model.OrderId}&orderInfo={model.OrderInfo}&returnUrl={returnUrl}&notifyUrl={notifyUrl}&extraData=";
            var signature = HmacSha256(secretKey, rawData);

            var payload = new
            {
                accessKey = accessKey,
                partnerCode = partnerCode,
                requestType = requestType,
                notifyUrl = notifyUrl,
                returnUrl = returnUrl,
                orderId = model.OrderId,
                amount = amount,
                orderInfo = model.OrderInfo,
                requestId = model.OrderId,
                extraData = "",
                signature = signature
            };

            using var client = new HttpClient();
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(endpoint, content);
            var responseBody = await response.Content.ReadAsStringAsync();

            var res = JsonSerializer.Deserialize<MomoCreatePaymentResponseModel>(responseBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new MomoCreatePaymentResponseModel();

            return res;
        }

        public MomoExecuteResponseModel PaymentExecuteAsync(IQueryCollection collection)
        {
            return new MomoExecuteResponseModel
            {
                Amount = collection["amount"],
                OrderId = collection["orderId"],
                OrderInfo = collection["orderInfo"]
            };
        }
    }
}


