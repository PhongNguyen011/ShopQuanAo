using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopQuanAo.Models
{
    public class ShippingFee
    {
        public int Id { get; set; }

        /// <summary>
        /// Tên tỉnh/thành phố (lấy từ API địa chỉ)
        /// </summary>
        [Required]
        [StringLength(100)]
        public string ProvinceName { get; set; } = string.Empty;

        /// <summary>
        /// Phí ship cố định cho tỉnh/thành này
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Phí ship (đ)")]
        [Range(0, 999999999, ErrorMessage = "Phí ship không hợp lệ")]
        public decimal Fee { get; set; }

        /// <summary>
        /// Mô tả (ví dụ: "Nội thành", "Ngoại thành", "Miền Bắc", etc.)
        /// </summary>
        [StringLength(200)]
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Display(Name = "Đang hoạt động")]
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
