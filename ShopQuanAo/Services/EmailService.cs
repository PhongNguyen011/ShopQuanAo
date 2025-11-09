using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace ShopQuanAo.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            try
            {
                // Lấy cấu hình email
                var senderEmail = _configuration["EmailSettings:SenderEmail"];
                var password = _configuration["EmailSettings:Password"];
                var senderName = _configuration["EmailSettings:SenderName"] ?? "Shop Quần Áo";
                
                // Kiểm tra cấu hình email
                if (string.IsNullOrEmpty(senderEmail) || string.IsNullOrEmpty(password) || 
                    senderEmail == "your-email@gmail.com" || password == "your-app-password")
                {
                    _logger.LogWarning("==================================================");
                    _logger.LogWarning("CẢNH BÁO: Cấu hình email chưa được thiết lập!");
                    _logger.LogWarning("Vui lòng cập nhật EmailSettings trong appsettings.json:");
                    _logger.LogWarning("- SenderEmail: Email Gmail của bạn");
                    _logger.LogWarning("- Password: App Password từ Google");
                    _logger.LogWarning("Hướng dẫn tạo App Password:");
                    _logger.LogWarning("1. Vào Google Account > Security");
                    _logger.LogWarning("2. Bật 2-Step Verification");
                    _logger.LogWarning("3. Tạo App Password tại App passwords section");
                    _logger.LogWarning("==================================================");
                    
                    // Chỉ log thông tin trong development mode
                    Console.WriteLine("\n====== THÔNG TIN EMAIL (Chưa gửi - Cần cấu hình) ======");
                    Console.WriteLine($"Người nhận: {toEmail}");
                    Console.WriteLine($"Tiêu đề: {subject}");
                    Console.WriteLine($"Link xác thực có trong email body");
                    Console.WriteLine("======================================================\n");
                    
                    return;
                }

                // Tạo email
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(senderName, senderEmail));
                email.To.Add(MailboxAddress.Parse(toEmail));
                email.Subject = subject;
                email.Body = new TextPart(TextFormat.Html) { Text = message };

                // Gửi email qua SMTP
                using var smtp = new SmtpClient();
                
                // Kết nối đến SMTP server
                await smtp.ConnectAsync(
                    _configuration["EmailSettings:SmtpServer"] ?? "smtp.gmail.com",
                    int.Parse(_configuration["EmailSettings:Port"] ?? "587"),
                    SecureSocketOptions.StartTls
                );

                // Xác thực
                await smtp.AuthenticateAsync(senderEmail, password);
                
                // Gửi email
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                _logger.LogInformation($"✓ Email đã được gửi thành công đến {toEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"✗ Lỗi khi gửi email đến {toEmail}");
                _logger.LogError($"Chi tiết lỗi: {ex.Message}");
                
                // Không throw exception để không làm gián đoạn quá trình đăng ký
                // Người dùng vẫn có thể đăng ký, nhưng cần xác thực email sau
                Console.WriteLine($"\n⚠ CẢNH BÁO: Không thể gửi email xác thực đến {toEmail}");
                Console.WriteLine($"Lỗi: {ex.Message}\n");
            }
        }

        public Task SendAsync(string v, string subject, string html)
            => SendEmailAsync(v, subject, html);
    }
}

