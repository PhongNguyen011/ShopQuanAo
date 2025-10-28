using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopQuanAo.Models
{
    public enum DiscountType { Percentage = 0, FixedAmount = 1 }
    public enum CouponScope { All = 0, CategoryOnly = 1 }

    public class Coupon
    {
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string Code { get; set; } = string.Empty; // luôn lưu UPPER

        public DiscountType DiscountType { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue)]
        public decimal DiscountValue { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? MinOrderAmount { get; set; }

        [Required]
        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        public DateTime? EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        // Phạm vi áp dụng
        public CouponScope Scope { get; set; } = CouponScope.All;

        /// <summary>CSV các category hợp lệ khi Scope = CategoryOnly (VD: "men,women")</summary>
        [StringLength(1000)]
        public string? AllowedCategoriesCsv { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
