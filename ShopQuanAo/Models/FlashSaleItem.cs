using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopQuanAo.Models
{
    public class FlashSaleItem
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn sản phẩm")]
        public int ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product? Product { get; set; }

        [Required(ErrorMessage = "Nhập giá Flash")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Giá Flash (đ)")]
        public decimal FlashPrice { get; set; }

        [Required(ErrorMessage = "Chọn thời gian kết thúc")]
        [Display(Name = "Kết thúc lúc")]
        public DateTime EndTime { get; set; }

        [Display(Name = "Hiển thị")]
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
