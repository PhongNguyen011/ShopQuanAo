namespace ShopQuanAo.Services
{
    public interface IEmailService
    {
        Task SendAsync(string v, string subject, string html);
        Task SendEmailAsync(string toEmail, string subject, string message);
    }
}



