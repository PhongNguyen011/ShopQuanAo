using System;

namespace ShopQuanAo.Models
{
    public class MomoTransaction
    {
        public int Id { get; set; }

        // MoMo identifiers
        public string OrderId { get; set; } = string.Empty;
        public string? RequestId { get; set; }
        public string? TransactionId { get; set; } // transId

        // Payment info
        public decimal Amount { get; set; }
        public string OrderInfo { get; set; } = string.Empty;

        // Response/meta
        public int? ResultCode { get; set; }
        public string? Message { get; set; }
        public string? PayUrl { get; set; }
        public string? QrCodeUrl { get; set; }
        public string? Deeplink { get; set; }
        public string? Signature { get; set; }

        // Audit
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Business status: pending/success/failed
        public string Status { get; set; } = "pending";
    }
}


