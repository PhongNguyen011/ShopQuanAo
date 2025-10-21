using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopQuanAo.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
        [StringLength(200)]
        [Display(Name = "Tên Sản Phẩm")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Mô Tả")]
        [StringLength(1000)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá")]
        [Display(Name = "Giá")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        public decimal Price { get; set; }

        [Display(Name = "Giá Cũ")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? OldPrice { get; set; }

        [Display(Name = "Hình Ảnh")]
        [StringLength(500)]
        public string? ImageUrl { get; set; }

        [Display(Name = "Danh Mục")]
        [StringLength(100)]
        public string? Category { get; set; }

        [Display(Name = "Có Sẵn")]
        public bool IsAvailable { get; set; } = true;

        [Display(Name = "Nổi Bật")]
        public bool IsFeatured { get; set; } = false;

        [Display(Name = "Giảm Giá")]
        public bool IsOnSale { get; set; } = false;

        [Display(Name = "Số Lượng Tồn Kho")]
        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; } = 0;

        [Display(Name = "Ngày Tạo")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Display(Name = "Ngày Cập Nhật")]
        public DateTime? UpdatedDate { get; set; }
    }
}

