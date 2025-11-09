using Microsoft.AspNetCore.Mvc;
using ShopQuanAo.Data;
using ShopQuanAo.Models;
using ShopQuanAo.Models.ViewModels;
using ShopQuanAo.Services;

namespace WebApplication1.Controllers
{
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _email; // đã có trong Services của bạn

        public ContactController(ApplicationDbContext context, IEmailService email)
        {
            _context = context;
            _email = email;
        }

        [HttpGet]
        public IActionResult Index() => View(new ContactFormViewModel());

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Send(ContactFormViewModel vm)
        {
            if (!ModelState.IsValid) return View("Index", vm);

            var entity = new ContactMessage
            {
                Name = vm.Name.Trim(),
                Email = vm.Email.Trim(),
                Website = string.IsNullOrWhiteSpace(vm.Website) ? null : vm.Website.Trim(),
                Message = vm.Message.Trim()
            };
            _context.ContactMessages.Add(entity);
            await _context.SaveChangesAsync();

            // (tuỳ chọn) báo Admin — dùng appsettings: "Admin:Email"
            var config = HttpContext.RequestServices
                .GetService(typeof(Microsoft.Extensions.Configuration.IConfiguration))
                as Microsoft.Extensions.Configuration.IConfiguration;
            var adminEmail = config?["Admin:Email"];
            if (!string.IsNullOrWhiteSpace(adminEmail))
            {
                var subject = "[ShopQuanAo] Liên hệ mới";
                var html = $@"
                    <p><b>Người gửi:</b> {System.Net.WebUtility.HtmlEncode(entity.Name)}</p>
                    <p><b>Email:</b> {System.Net.WebUtility.HtmlEncode(entity.Email)}</p>
                    {(string.IsNullOrEmpty(entity.Website) ? "" : "<p><b>Website:</b> " + System.Net.WebUtility.HtmlEncode(entity.Website) + "</p>")}
                    <p><b>Nội dung:</b></p>
                    <p>{System.Net.WebUtility.HtmlEncode(entity.Message).Replace("\n", "<br/>")}</p>
                    <hr/><p>Thời gian: {entity.CreatedAt:dd/MM/yyyy HH:mm}</p>";
                await _email.SendAsync(adminEmail!, subject, html);
            }

            return RedirectToAction(nameof(ThankYou));
        }

        [HttpGet]
        public IActionResult ThankYou()
        {
            return View();
        }
    }
}
