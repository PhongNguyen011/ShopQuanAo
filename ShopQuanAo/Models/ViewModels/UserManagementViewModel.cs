using System.ComponentModel.DataAnnotations;

namespace ShopQuanAo.Models.ViewModels
{
    public class UserManagementViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }

    public class EditUserViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Email đã xác thực")]
        public bool EmailConfirmed { get; set; }

        [Display(Name = "Tài khoản hoạt động")]
        public bool IsActive { get; set; }

        [Display(Name = "Vai trò")]
        public List<string> SelectedRoles { get; set; } = new List<string>();

        // Danh sách tất cả các vai trò trong hệ thống
        public List<RoleSelectionViewModel> AllRoles { get; set; } = new List<RoleSelectionViewModel>();
    }

    public class RoleSelectionViewModel
    {
        public string RoleId { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }
}

