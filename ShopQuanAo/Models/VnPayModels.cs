namespace ShopQuanAo.Models
{
    public class PaymentInformationModel
    {
        public string OrderType { get; set; } = "other";
        public double Amount { get; set; }
        public string OrderDescription { get; set; } = "";
        public string Name { get; set; } = "";
    }

    public class PaymentResponseModel
    {
        public bool Success { get; set; }
        public string OrderId { get; set; } = "";
        public string PaymentId { get; set; } = "";
        public string VnPayResponseCode { get; set; } = "";
        public string Token { get; set; } = "";
        public string OrderDescription { get; set; } = "";
    }
}