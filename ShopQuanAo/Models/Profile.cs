using System.ComponentModel.DataAnnotations;

namespace ShopQuanAo.Models
{
    public class Profile
    {
        public string UserId { get; set; } = "";
        public string UserName { get; set; } = "";
        public string Email { get; set; } = "";

        [Display(Name = "Số điện thoại")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Họ và Tên"), StringLength(100)]
        public string? FullName { get; set; }

        [Display(Name = "Địa Chỉ"), StringLength(500)]
        public string? Address { get; set; }

        [Display(Name = "Ngày Sinh"), DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(255)]
        public string? AvatarFileName { get; set; }

    }
}
