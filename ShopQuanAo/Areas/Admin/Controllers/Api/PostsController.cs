using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopQuanAo.Data;
using ShopQuanAo.Models;
using System.IO;

namespace ShopQuanAo.Areas.Admin.Controllers.Api
{
    /// <summary>
    /// API quản lý bài viết (Posts) dành cho Admin.
    /// </summary>
    [Area("Admin")]
    [ApiController]
    [Route("api/admin/[controller]")]
    [Authorize(Roles = "Admin")]
    [Produces("application/json")]
    public class PostsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        private static readonly string[] AllowedExts = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxBytes = 2 * 1024 * 1024; // 2MB
        private const string UploadFolder = "images/posts"; // trong wwwroot

        public PostsController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        /// <summary>
        /// Danh sách bài viết (có tìm kiếm theo tiêu đề / nội dung).
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Post>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Post>>> GetList([FromQuery] string? q)
        {
            var query = _context.Posts.AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(p =>
                    p.Title.Contains(q) ||
                    (p.Content != null && p.Content.Contains(q)));
            }

            var posts = await query
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return Ok(posts);
        }

        /// <summary>
        /// Chi tiết 1 bài viết theo Id.
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(Post), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Post>> GetById(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null) return NotFound(new { message = "Không tìm thấy bài viết." });
            return Ok(post);
        }

        /// <summary>
        /// Tạo bài viết mới (hỗ trợ upload ảnh thumbnail).
        /// </summary>
        [HttpPost]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(Post), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Post>> Create([FromForm] AdminPostCreateUpdateDto model)
        {
            if (string.IsNullOrWhiteSpace(model.Title))
                ModelState.AddModelError(nameof(model.Title), "Tiêu đề không được để trống.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var post = new Post
            {
                Title = model.Title.Trim(),
                Content = model.Content,
                Slug = GenerateSlug(model.Title),
                CreatedAt = DateTime.Now
            };

            if (model.ImageUpload != null)
            {
                var saveResult = await SaveImageAsync(model.ImageUpload);
                if (!saveResult.ok)
                {
                    ModelState.AddModelError(nameof(model.ImageUpload), saveResult.error!);
                    return BadRequest(ModelState);
                }

                post.Thumbnail = saveResult.fileName;
            }

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = post.Id }, post);
        }

        /// <summary>
        /// Cập nhật bài viết (có thể đổi thumbnail).
        /// </summary>
        [HttpPut("{id:int}")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(Post), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Post>> Update(int id, [FromForm] AdminPostCreateUpdateDto model)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null) return NotFound(new { message = "Không tìm thấy bài viết." });

            if (string.IsNullOrWhiteSpace(model.Title))
                ModelState.AddModelError(nameof(model.Title), "Tiêu đề không được để trống.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            post.Title = model.Title.Trim();
            post.Content = model.Content;
            post.Slug = GenerateSlug(model.Title);
            post.UpdatedAt = DateTime.Now;

            if (model.ImageUpload != null)
            {
                var saveResult = await SaveImageAsync(model.ImageUpload);
                if (!saveResult.ok)
                {
                    ModelState.AddModelError(nameof(model.ImageUpload), saveResult.error!);
                    return BadRequest(ModelState);
                }

                // Xoá ảnh cũ nếu có
                if (!string.IsNullOrEmpty(post.Thumbnail))
                {
                    DeleteImage(post.Thumbnail);
                }
                post.Thumbnail = saveResult.fileName;
            }

            await _context.SaveChangesAsync();
            return Ok(post);
        }

        /// <summary>
        /// Xoá bài viết.
        /// </summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null) return NotFound(new { message = "Không tìm thấy bài viết." });

            if (!string.IsNullOrEmpty(post.Thumbnail))
            {
                DeleteImage(post.Thumbnail);
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ================== Helper giống controller MVC ==================

        private static string GenerateSlug(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;
            var s = input.Trim().ToLowerInvariant();
            s = s.Replace("đ", "d");
            s = System.Text.RegularExpressions.Regex.Replace(s, @"\s+", "-");
            s = System.Text.RegularExpressions.Regex.Replace(s, @"[^a-z0-9\-]", "");
            s = System.Text.RegularExpressions.Regex.Replace(s, @"-+", "-").Trim('-');
            return s;
        }

        private async Task<(bool ok, string? fileName, string? error)> SaveImageAsync(IFormFile file)
        {
            if (file.Length > MaxBytes)
                return (false, null, $"Ảnh quá lớn (> {MaxBytes / (1024 * 1024)}MB).");

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExts.Contains(ext))
                return (false, null, "Chỉ chấp nhận .jpg, .jpeg, .png, .webp");

            var root = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var folderPath = Path.Combine(root, UploadFolder.Replace("/", Path.DirectorySeparatorChar.ToString()));
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var safeName = Path.GetFileNameWithoutExtension(file.FileName);
            safeName = System.Text.RegularExpressions.Regex.Replace(safeName, @"[^a-zA-Z0-9-_]", "");
            var uniqueName = $"{safeName}-{Guid.NewGuid():N}{ext}";

            var savePath = Path.Combine(folderPath, uniqueName);
            using (var stream = new FileStream(savePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return (true, uniqueName, null);
        }

        private void DeleteImage(string fileName)
        {
            var root = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var fullPath = Path.Combine(root, UploadFolder.Replace("/", Path.DirectorySeparatorChar.ToString()), fileName);
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }
    }

    /// <summary>
    /// DTO tạo / cập nhật bài viết (dùng cho API Admin, hỗ trợ upload file).
    /// </summary>
    public class AdminPostCreateUpdateDto
    {
        /// <summary>Tiêu đề bài viết.</summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>Nội dung HTML / text.</summary>
        public string? Content { get; set; }

        /// <summary>Ảnh upload (multipart/form-data).</summary>
        public IFormFile? ImageUpload { get; set; }
    }
}
