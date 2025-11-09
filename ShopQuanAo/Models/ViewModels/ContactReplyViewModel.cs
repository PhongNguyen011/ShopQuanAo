using System.ComponentModel.DataAnnotations;

namespace ShopQuanAo.Models.ViewModels
{
    public class ContactReplyViewModel
    {
        public int Id { get; set; }

        // hiển thị lại thông tin gốc
        public string FromName { get; set; } = "";
        public string FromEmail { get; set; } = "";
        public string? FromWebsite { get; set; }
        public string OriginalMessage { get; set; } = "";

        // phần soạn thư
        [Required, StringLength(250)]
        public string Subject { get; set; } = "";
        [Required, StringLength(8000)]
        public string Body { get; set; } = "";
    }
}
