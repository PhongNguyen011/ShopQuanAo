using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ShopQuanAo.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "Họ và Tên")]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "Địa Chỉ")]
        [StringLength(500)]
        public string? Address { get; set; }

        [Display(Name = "Ngày Sinh")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Ngày Tạo")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Display(Name = "Ngày Cập Nhật")]
        public DateTime? UpdatedDate { get; set; }

        [Display(Name = "Trạng Thái")]
        public bool IsActive { get; set; } = true;
    }
}



