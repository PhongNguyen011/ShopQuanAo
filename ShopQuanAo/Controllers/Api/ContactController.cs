using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopQuanAo.Data;
using ShopQuanAo.Models;
using ShopQuanAo.Models.ViewModels;
using ShopQuanAo.Services;

namespace ShopQuanAo.Controllers.Api
{
    /// <summary>
    /// API nhận liên hệ từ khách (contact form) và xem lại danh sách (cho admin).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ContactController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _email;
        private readonly IConfiguration _config;

        public ContactController(ApplicationDbContext context, IEmailService email, IConfiguration config)
        {
            _context = context;
            _email = email;
            _config = config;
        }

        /// <summary>Gửi liên hệ mới.</summary>
        [HttpPost]
        [ProducesResponseType(typeof(ContactMessage), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ContactMessage>> Send([FromBody] ContactFormViewModel vm)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entity = new ContactMessage
            {
                Name = vm.Name.Trim(),
                Email = vm.Email.Trim(),
                Website = string.IsNullOrWhiteSpace(vm.Website) ? null : vm.Website.Trim(),
                Message = vm.Message.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            _context.ContactMessages.Add(entity);
            await _context.SaveChangesAsync();

            // Tuỳ chọn: gửi email báo admin
            var adminEmail = _config["Admin:Email"];
            if (!string.IsNullOrWhiteSpace(adminEmail))
            {
                try
                {
                    var subject = "[ShopQuanAo] Liên hệ mới";
                    var html = $@"
                        <p><b>Người gửi:</b> {System.Net.WebUtility.HtmlEncode(entity.Name)}</p>
                        <p><b>Email:</b> {System.Net.WebUtility.HtmlEncode(entity.Email)}</p>
                        {(string.IsNullOrEmpty(entity.Website) ? "" : "<p><b>Website:</b> " + System.Net.WebUtility.HtmlEncode(entity.Website) + "</p>")}
                        <p><b>Nội dung:</b></p>
                        <p>{System.Net.WebUtility.HtmlEncode(entity.Message).Replace("\n", "<br/>")}</p>
                        <hr/><p>Thời gian: {entity.CreatedAt:dd/MM/yyyy HH:mm}</p>";
                    await _email.SendAsync(adminEmail, subject, html);
                }
                catch
                {
                    // tránh văng lỗi API nếu gửi mail thất bại
                }
            }

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
        }

        /// <summary>Danh sách tất cả contact message (dùng cho admin).</summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ContactMessage>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ContactMessage>>> GetAll()
        {
            var list = await _context.ContactMessages
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return Ok(list);
        }

        /// <summary>Lấy chi tiết 1 contact theo Id.</summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ContactMessage), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ContactMessage>> GetById(int id)
        {
            var c = await _context.ContactMessages.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (c == null) return NotFound();
            return Ok(c);
        }
    }
}
