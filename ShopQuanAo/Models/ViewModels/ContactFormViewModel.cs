using System.ComponentModel.DataAnnotations;

namespace ShopQuanAo.Models.ViewModels
{
    public class ContactFormViewModel
    {
        [Required, StringLength(120)]
        public string Name { get; set; } = "";
        [Required, EmailAddress, StringLength(200)]
        public string Email { get; set; } = "";
        [Url, StringLength(300)]
        public string? Website { get; set; }
        [Required, StringLength(4000)]
        public string Message { get; set; } = "";
    }
}
