using System.ComponentModel.DataAnnotations;

namespace ShopQuanAo.Models
{
    public class ContactMessage
    {
        public int Id { get; set; }

        [Required, StringLength(120)]
        public string Name { get; set; } = "";

        [Required, EmailAddress, StringLength(200)]
        public string Email { get; set; } = "";

        [Url, StringLength(300)]
        public string? Website { get; set; }

        [Required, StringLength(4000)]
        public string Message { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // trạng thái đọc
        public bool IsRead { get; set; } = false;
        public DateTime? ReadAt { get; set; }

        // thông tin trả lời từ Admin
        public bool IsReplied { get; set; } = false;
        public DateTime? RepliedAt { get; set; }
        [StringLength(200)]
        public string? RepliedBy { get; set; }     // username/email admin
        [StringLength(250)]
        public string? ReplySubject { get; set; }
        [StringLength(8000)]
        public string? ReplyBody { get; set; }
    }
}
