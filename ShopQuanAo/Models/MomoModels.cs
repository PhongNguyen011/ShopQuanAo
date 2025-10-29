namespace ShopQuanAo.Models
{
    // Dùng để submit tạo giao dịch
    public class OrderInfoModel
    {
        public string FullName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string OrderInfo { get; set; } = string.Empty;
        public string OrderId { get; set; } = string.Empty;
    }

    // Phản hồi khi tạo thanh toán (MoMo trả về)
    public class MomoCreatePaymentResponseModel
    {
        public string RequestId { get; set; } = string.Empty;
        public int ErrorCode { get; set; }
        public string OrderId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string PayUrl { get; set; } = string.Empty;
        public string Signature { get; set; } = string.Empty;
        public string QrCodeUrl { get; set; } = string.Empty;
        public string Deeplink { get; set; } = string.Empty;
    }

    // Dùng cho callback/return để hiển thị
    public class MomoExecuteResponseModel
    {
        public string OrderId { get; set; } = string.Empty;
        public string Amount { get; set; } = string.Empty;
        public string OrderInfo { get; set; } = string.Empty;
    }

    // Options cấu hình từ appsettings
    public class MomoOptionModel
    {
        public string MomoApiUrl { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public string AccessKey { get; set; } = string.Empty;
        public string ReturnUrl { get; set; } = string.Empty;
        public string NotifyUrl { get; set; } = string.Empty;
        public string PartnerCode { get; set; } = string.Empty;
        public string RequestType { get; set; } = string.Empty;
    }
}


