using System.ComponentModel.DataAnnotations;

namespace ShopQuanAo.Models
{
    public class Order
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string OrderCode { get; set; } = string.Empty;
        
        [Required]
        [StringLength(200)]
        public string CustomerName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;
        
        [Required]
        [StringLength(500)]
        public string Address { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string? Note { get; set; }
        
        [Required]
        [StringLength(20)]
        public string PaymentMethod { get; set; } = "COD"; // COD, VNPAY, MOMO
        
        public decimal Subtotal { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
        
        [StringLength(50)]
        public string? CouponCode { get; set; }
        
        [StringLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Confirmed, Shipping, Delivered, Cancelled
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        
        // VNPay specific fields
        [StringLength(100)]
        public string? VnPayTransactionId { get; set; }
        
        [StringLength(100)]
        public string? VnPayOrderId { get; set; }
        
        [StringLength(10)]
        public string? VnPayResponseCode { get; set; }
        
        public DateTime? VnPayPaidAt { get; set; }
        
        // Navigation properties
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
    
    public class OrderItem
    {
        public int Id { get; set; }
        
        public int OrderId { get; set; }
        public virtual Order Order { get; set; } = null!;
        
        [Required]
        [StringLength(200)]
        public string ProductName { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? ProductImage { get; set; }
        
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal { get; set; }
    }
}
