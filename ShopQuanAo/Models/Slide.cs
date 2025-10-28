using System.ComponentModel.DataAnnotations;

namespace ShopQuanAo.Models
{
    public class Slide
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(255)]
        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now; 
    }
}
