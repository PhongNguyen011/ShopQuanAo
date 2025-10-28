using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using ShopQuanAo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShopQuanAo.Services
{
    public class VnPayServiceNew : IVnPayService
    {
        private readonly IConfiguration _cfg;

        public VnPayServiceNew(IConfiguration cfg)
        {
            _cfg = cfg;
        }

        private static string Enc(string s)
        {
            return Uri.EscapeDataString(s ?? "").Replace("%20", "+");
        }

        private static string Hmac512(string key, string data)
        {
            using var h = new System.Security.Cryptography.HMACSHA512(Encoding.UTF8.GetBytes(key));
            return BitConverter.ToString(h.ComputeHash(Encoding.UTF8.GetBytes(data))).Replace("-", "").ToLowerInvariant();
        }

        public string CreatePaymentUrl(PaymentInformationModel m, HttpContext ctx)
        {
            var baseUrl = _cfg["VnPay:BaseUrl"]!;
            var tmn = _cfg["VnPay:TmnCode"]!;
            var secret = _cfg["VnPay:HashSecret"]!.Trim();
            var ret = _cfg["VnPay:ReturnUrl"]!;
            var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                       TimeZoneInfo.FindSystemTimeZoneById(_cfg["TimeZoneId"] ?? "SE Asia Standard Time"));
            var orderId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            var amt = ((long)Math.Round(m.Amount)) * 100;

            var dict = new SortedDictionary<string, string>(StringComparer.Ordinal)
            {
                ["vnp_Version"] = _cfg["VnPay:Version"] ?? "2.1.0",
                ["vnp_Command"] = "pay",
                ["vnp_TmnCode"] = tmn,
                ["vnp_Amount"] = amt.ToString(),
                ["vnp_CreateDate"] = now.ToString("yyyyMMddHHmmss"),
                ["vnp_CurrCode"] = _cfg["VnPay:CurrCode"] ?? "VND",
                ["vnp_IpAddr"] = ctx.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1",
                ["vnp_Locale"] = _cfg["VnPay:Locale"] ?? "vn",
                ["vnp_OrderInfo"] = $"{m.Name} {m.OrderDescription} {m.Amount}",
                ["vnp_OrderType"] = m.OrderType,
                ["vnp_ReturnUrl"] = ret,
                ["vnp_TxnRef"] = orderId
            };

            var raw = string.Join("&", dict.Select(kv => $"{Enc(kv.Key)}={Enc(kv.Value)}"));
            var hash = Hmac512(secret, raw);
            return $"{baseUrl}?{raw}&vnp_SecureHash={hash}";
        }

        public PaymentResponseModel PaymentExecute(IQueryCollection q)
        {
            if (!q.TryGetValue("vnp_SecureHash", out var recv) || string.IsNullOrWhiteSpace(recv))
                return new PaymentResponseModel { Success = false };

            var dict = q.Where(p => p.Key.StartsWith("vnp_", StringComparison.OrdinalIgnoreCase))
                        .ToDictionary(p => p.Key, p => p.Value.ToString(), StringComparer.Ordinal);
            dict.Remove("vnp_SecureHash");
            dict.Remove("vnp_SecureHashType");

            var raw = string.Join("&", dict.OrderBy(k => k.Key, StringComparer.Ordinal)
                                           .Select(kv => $"{Enc(kv.Key)}={Enc(kv.Value)}"));
            var expected = Hmac512((_cfg["VnPay:HashSecret"] ?? "").Trim(), raw);

            var ok = string.Equals(recv.ToString(), expected, StringComparison.OrdinalIgnoreCase);
            return new PaymentResponseModel
            {
                Success = ok && dict.TryGetValue("vnp_ResponseCode", out var rc) && rc == "00",
                OrderId = dict.GetValueOrDefault("vnp_TxnRef", ""),
                PaymentId = dict.GetValueOrDefault("vnp_TransactionNo", ""),
                VnPayResponseCode = dict.GetValueOrDefault("vnp_ResponseCode", ""),
                Token = recv,
                OrderDescription = dict.GetValueOrDefault("vnp_OrderInfo", "")
            };
        }
    }
}
