using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopQuanAo.Models
{
    public class Post
    {
        public int Id { get; set; }

        [Display(Name = "Tiêu đề")]
        [Required, StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Slug")]
        [StringLength(220)]
        public string? Slug { get; set; }

        [Display(Name = "Nội dung")]
        [Required]
        public string Content { get; set; } = string.Empty;

        [Display(Name = "Ảnh đại diện")]
        [StringLength(255)]
        public string? Thumbnail { get; set; }   // 👈 thêm cột DB

        [NotMapped]
        public IFormFile? ImageUpload { get; set; } // 👈 chỉ dùng để bind file

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Ngày cập nhật")]
        public DateTime? UpdatedAt { get; set; }
    }
}
