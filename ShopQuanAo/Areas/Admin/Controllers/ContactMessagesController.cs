using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopQuanAo.Data;
using ShopQuanAo.Models;
using ShopQuanAo.Models.ViewModels;
using ShopQuanAo.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ShopQuanAo.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ContactMessagesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _email;

        public ContactMessagesController(ApplicationDbContext context, IEmailService email)
        {
            _context = context;
            _email = email;
        }

        public async Task<IActionResult> Index(string? q, string filter = "all")
        {
            var query = _context.ContactMessages.AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(x => x.Name.Contains(q) || x.Email.Contains(q) || x.Message.Contains(q));
            }
            if (filter == "unread") query = query.Where(x => !x.IsRead);
            if (filter == "unreplied") query = query.Where(x => !x.IsReplied);

            var list = await query
                .OrderByDescending(x => !x.IsRead || !x.IsReplied)
                .ThenByDescending(x => x.CreatedAt)
                .ToListAsync();

            ViewBag.Filter = filter;
            ViewBag.Keyword = q;
            return View(list);
        }

        public async Task<IActionResult> Details(int id)
        {
            var m = await _context.ContactMessages.FindAsync(id);
            if (m == null) return NotFound();

            if (!m.IsRead)
            {
                m.IsRead = true;
                m.ReadAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            return View(m);
        }

        [HttpGet]
        public async Task<IActionResult> Reply(int id)
        {
            var m = await _context.ContactMessages.FindAsync(id);
            if (m == null) return NotFound();

            var vm = new ContactReplyViewModel
            {
                Id = m.Id,
                FromName = m.Name,
                FromEmail = m.Email,
                FromWebsite = m.Website,
                OriginalMessage = m.Message,
                Subject = m.IsReplied && !string.IsNullOrEmpty(m.ReplySubject)
                          ? m.ReplySubject!
                          : $"Admin Reply {m.Name}"
            };
            return View(vm);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Reply(ContactReplyViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var m = await _context.ContactMessages.FindAsync(vm.Id);
            if (m == null) return NotFound();

            // Lấy tên shop từ appsettings (tùy chọn)
            var cfg = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var shopName = cfg["Shop:Name"] ?? "Shop Quần Áo";
            var shopUrl = cfg["Shop:Url"] ?? ""; // nếu có

            // Encode nội dung admin gõ + xuống dòng -> <br>
            string Safe(string? s) => System.Net.WebUtility.HtmlEncode(s ?? "");
            var safeBody = Safe(vm.Body).Replace("\n", "<br/>").Replace("\r", "");

            // Trích nguyên văn nội dung gốc của khách (quote block)
            var original = Safe(m.Message).Replace("\n", "<br/>").Replace("\r", "");

            // ========== TEMPLATE EMAIL TRẢ LỜI (HTML) ==========
            var html = $@"
            <!doctype html>
            <html lang=""vi""><head>
            <meta charset=""utf-8"">
            <meta name=""viewport"" content=""width=device-width,initial-scale=1"">
            <title>{Safe(vm.Subject)}</title>
            <style>
            @media (max-width:600px){{ .wrap{{padding:16px !important}} .btn{{display:block !important;width:100% !important}} }}
            </style>
            </head>
            <body style=""margin:0;padding:0;background:#f5f6f8"">
              <table role=""presentation"" width=""100%"" cellspacing=""0"" cellpadding=""0"">
                <tr><td align=""center"" style=""padding:24px 12px"">

                  <table role=""presentation"" width=""640"" style=""max-width:640px;background:#ffffff;border-radius:12px;overflow:hidden"">
                    <tr><td class=""wrap"" style=""padding:28px 32px;font-family:Segoe UI,Roboto,Helvetica,Arial,sans-serif;color:#111827;font-size:16px;line-height:1.6"">

                      <!-- Phần trả lời của Admin -->
                        <p style=""margin:0 0 10px 0;font-size:15px"">
                            Chào <b>{Safe(m.Name)}</b>,</p> <p style=""margin:0 0 16px 0;font-size:12px;color:#6b7280""> 
                            Đây là phản hồi từ bộ phận hỗ trợ {Safe(shopName)}: 
                        </p>
                      <div style=""margin:0 0 8px 0"">
                        {safeBody}
                      </div>
                      <p style=""margin:6px 0 0 0;text-align:right;font-weight:600"">Gửi bạn <b>{Safe(m.Name)}</b></p>

                      <!-- Khoảng cách trước nội dung ban đầu -->
                      <div style=""height:20px""></div>

                      <p style=""margin:0 0 8px 0;font-size:15px;color:#6b7280"">Nội dung liên hệ ban đầu của bạn:</p>
                      <blockquote style=""margin:0;border-left:4px solid #e5e7eb;padding-left:12px;color:#374151;font-size:15px;line-height:1.6"">
                        {original}
                      </blockquote>

                      {(string.IsNullOrEmpty(shopUrl) ? "" : $@"<div style=""text-align:center;margin-top:20px"">
                        <a href=""{shopUrl}"" class=""btn"" style=""background:#0d6efd;color:#fff;text-decoration:none;padding:10px 16px;border-radius:8px;display:inline-block;font-weight:600"">
                          Truy cập {Safe(shopName)}
                        </a>
                      </div>")}

                      <hr style=""border:none;border-top:1px solid #e5e7eb;margin:20px 0"">
                      <p style=""margin:0;color:#6b7280;font-size:12px"">
                        Bạn nhận được email này vì đã gửi liên hệ tới {Safe(shopName)}.
                      </p>
                    </td></tr>
                  </table>

                  <div style=""font-family:Segoe UI,Roboto,Helvetica,Arial,sans-serif;color:#6b7280;font-size:12px;margin-top:12px"">
                    © {DateTime.Now:yyyy} {Safe(shopName)}
                  </div>

                </td></tr>
              </table>
            </body></html>";

            // ===================================================

            // Gửi email HTML (EmailService của bạn đã TextFormat.Html — OK)
            await _email.SendEmailAsync(m.Email, vm.Subject, html);

            // Lưu trạng thái trả lời
            m.IsReplied = true;
            m.RepliedAt = DateTime.Now;
            m.RepliedBy = User?.Identity?.Name ?? "admin";
            m.ReplySubject = vm.Subject;
            m.ReplyBody = vm.Body; // lưu bản gốc admin gõ (plain)
            await _context.SaveChangesAsync();

            TempData["msg"] = "Đã gửi phản hồi cho khách.";
            return RedirectToAction(nameof(Details), new { id = m.Id });
        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var m = await _context.ContactMessages.FindAsync(id);
            if (m == null) return NotFound();
            _context.ContactMessages.Remove(m);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
