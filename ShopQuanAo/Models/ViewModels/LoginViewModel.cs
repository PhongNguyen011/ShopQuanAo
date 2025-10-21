using System.ComponentModel.DataAnnotations;

namespace ShopQuanAo.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật Khẩu")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Ghi Nhớ Tài Khoản")]
        public bool RememberMe { get; set; }
    }
}



