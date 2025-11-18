using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ShopQuanAo.Data;
using ShopQuanAo.Models;
using ShopQuanAo.Models.ViewModels;
using ShopQuanAo.Services;

namespace ShopQuanAo.Areas.Admin.Controllers.Api
{
    /// <summary>
    /// API quản lý liên hệ khách hàng (ContactMessages) dành cho Admin.
    /// </summary>
    [Area("Admin")]
    [ApiController]
    [Route("api/admin/[controller]")]
    [Authorize(Roles = "Admin")]
    [Produces("application/json")]
    public class ContactMessagesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _email;
        private readonly IConfiguration _config;

        public ContactMessagesController(
            ApplicationDbContext context,
            IEmailService email,
            IConfiguration config)
        {
            _context = context;
            _email = email;
            _config = config;
        }

        /// <summary>
        /// Danh sách contact messages (có tìm kiếm, filter unread/unreplied).
        /// </summary>
        /// <param name="q">Từ khóa tìm kiếm theo tên/email/nội dung.</param>
        /// <param name="filter">all | unread | unreplied.</param>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ContactMessage>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ContactMessage>>> GetList(
            [FromQuery] string? q,
            [FromQuery] string filter = "all")
        {
            var query = _context.ContactMessages.AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(x =>
                    x.Name.Contains(q) ||
                    x.Email.Contains(q) ||
                    x.Message.Contains(q));
            }

            if (filter == "unread")
                query = query.Where(x => !x.IsRead);

            if (filter == "unreplied")
                query = query.Where(x => !x.IsReplied);

            var list = await query
                .OrderByDescending(x => !x.IsRead || !x.IsReplied)
                .ThenByDescending(x => x.CreatedAt)
                .ToListAsync();

            return Ok(list);
        }

        /// <summary>
        /// Lấy chi tiết 1 contact message theo Id, đồng thời đánh dấu đã đọc.
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ContactMessage), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ContactMessage>> GetDetail(int id)
        {
            var m = await _context.ContactMessages.FindAsync(id);
            if (m == null) return NotFound(new { message = "Không tìm thấy liên hệ." });

            if (!m.IsRead)
            {
                m.IsRead = true;
                m.ReadAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }

            return Ok(m);
        }

        /// <summary>
        /// Admin trả lời 1 contact message (gửi email cho khách + lưu log trả lời).
        /// </summary>
        [HttpPost("reply")]
        [ProducesResponseType(typeof(ContactMessage), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ContactMessage>> Reply([FromBody] ContactReplyApiDto vm)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var m = await _context.ContactMessages.FindAsync(vm.Id);
            if (m == null) return NotFound(new { message = "Không tìm thấy liên hệ." });

            // Lấy tên shop từ appsettings (tuỳ chọn)
            var shopName = _config["Shop:Name"] ?? "Shop Quần Áo";
            var shopUrl = _config["Shop:Url"] ?? "";

            // Encode nội dung admin gõ + xuống dòng -> <br>
            string Safe(string? s) => System.Net.WebUtility.HtmlEncode(s ?? "");
            var safeBody = Safe(vm.Body).Replace("\n", "<br/>").Replace("\r", "");
            var original = Safe(m.Message).Replace("\n", "<br/>").Replace("\r", "");

            var subject = string.IsNullOrWhiteSpace(vm.Subject)
                ? $"Admin Reply {m.Name}"
                : vm.Subject;

            // ========== TEMPLATE EMAIL TRẢ LỜI ==========

            var html = $@"
            <!doctype html>
            <html lang=""vi""><head>
            <meta charset=""utf-8"">
            <meta name=""viewport"" content=""width=device-width,initial-scale=1"">
            <title>{Safe(subject)}</title>
            <style>
            @media (max-width:600px){{ .wrap{{padding:16px !important}} .btn{{display:block !important;width:100% !important}} }}
            </style>
            </head>
            <body style=""margin:0;padding:0;background:#f5f6f8"">
              <table role=""presentation"" width=""100%"" cellspacing=""0"" cellpadding=""0"">
                <tr><td align=""center"" style=""padding:24px 12px"">

                  <table role=""presentation"" width=""640"" style=""max-width:640px;background:#ffffff;border-radius:12px;overflow:hidden"">
                    <tr><td class=""wrap"" style=""padding:28px 32px;font-family:Segoe UI,Roboto,Helvetica,Arial,sans-serif;color:#111827;font-size:16px;line-height:1.6"">

                      <p style=""margin:0 0 10px 0;font-size:15px"">
                          Chào <b>{Safe(m.Name)}</b>,</p>
                      <p style=""margin:0 0 16px 0;font-size:13px;color:#6b7280"">
                          Đây là phản hồi từ bộ phận hỗ trợ {Safe(shopName)}:
                      </p>

                      <div style=""margin:0 0 8px 0"">
                        {safeBody}
                      </div>
                      <p style=""margin:6px 0 0 0;text-align:right;font-weight:600"">
                        Gửi bạn <b>{Safe(m.Name)}</b>
                      </p>

                      <div style=""height:20px""></div>

                      <p style=""margin:0 0 8px 0;font-size:15px;color:#6b7280"">
                        Nội dung liên hệ ban đầu của bạn:
                      </p>
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

            // Gửi email
            await _email.SendEmailAsync(m.Email, subject, html);

            // Lưu trạng thái trả lời
            m.IsReplied = true;
            m.RepliedAt = DateTime.Now;
            m.RepliedBy = User?.Identity?.Name ?? "admin";
            m.ReplySubject = subject;
            m.ReplyBody = vm.Body;
            await _context.SaveChangesAsync();

            return Ok(m);
        }

        /// <summary>
        /// Xoá 1 contact message.
        /// </summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var m = await _context.ContactMessages.FindAsync(id);
            if (m == null) return NotFound(new { message = "Không tìm thấy liên hệ." });

            _context.ContactMessages.Remove(m);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

    /// <summary>
    /// DTO dùng cho API trả lời liên hệ (Admin reply).
    /// </summary>
    public class ContactReplyApiDto
    {
        /// <summary>Id của ContactMessage.</summary>
        public int Id { get; set; }

        /// <summary>Tiêu đề email gửi cho khách.</summary>
        public string? Subject { get; set; }

        /// <summary>Nội dung trả lời.</summary>
        public string Body { get; set; } = string.Empty;
    }
}
