using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using ShopQuanAo.Data;
using ShopQuanAo.Models;

namespace ShopQuanAo.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        private static readonly string[] AllowedExts = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxBytes = 2 * 1024 * 1024; // 2MB
        private const string UploadFolder = "images/posts"; // nằm trong wwwroot

        public PostsController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: /Admin/Posts
        public async Task<IActionResult> Index()
        {
            var posts = await _context.Posts
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
            return View(posts);
        }

        // GET: /Admin/Posts/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null) return NotFound();
            return View(post);
        }

        // GET: /Admin/Posts/Create
        public IActionResult Create() => View();

        // POST: /Admin/Posts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Post model)
        {
            if (!ModelState.IsValid) return View(model);

            model.Slug = GenerateSlug(model.Title);
            model.CreatedAt = DateTime.Now;

            // Kiểm tra trùng slug nếu bạn bật unique
            // var exists = await _context.Posts.AnyAsync(p => p.Slug == model.Slug);
            // if (exists) { ModelState.AddModelError("Title", "Tiêu đề tạo slug trùng."); return View(model); }

            if (model.ImageUpload != null)
            {
                var saveResult = await SaveImageAsync(model.ImageUpload);
                if (!saveResult.ok)
                {
                    ModelState.AddModelError("ImageUpload", saveResult.error!);
                    return View(model);
                }
                model.Thumbnail = saveResult.fileName;
            }

            _context.Posts.Add(model);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Thêm bài viết thành công!";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/Posts/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null) return NotFound();
            return View(post);
        }

        // POST: /Admin/Posts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Post model)
        {
            if (id != model.Id) return NotFound();
            if (!ModelState.IsValid) return View(model);

            var post = await _context.Posts.FindAsync(id);
            if (post == null) return NotFound();

            post.Title = model.Title;
            post.Content = model.Content;
            post.Slug = GenerateSlug(model.Title);
            post.UpdatedAt = DateTime.Now;

            if (model.ImageUpload != null)
            {
                var saveResult = await SaveImageAsync(model.ImageUpload);
                if (!saveResult.ok)
                {
                    ModelState.AddModelError("ImageUpload", saveResult.error!);
                    return View(model);
                }

                // Xoá ảnh cũ nếu có
                if (!string.IsNullOrEmpty(post.Thumbnail))
                {
                    DeleteImage(post.Thumbnail);
                }
                post.Thumbnail = saveResult.fileName;
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Cập nhật bài viết thành công!";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/Posts/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null) return NotFound();
            return View(post);
        }

        // POST: /Admin/Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                if (!string.IsNullOrEmpty(post.Thumbnail))
                {
                    DeleteImage(post.Thumbnail);
                }
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Xoá bài viết thành công!";
            }
            return RedirectToAction(nameof(Index));
        }

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
            // Validate kích thước
            if (file.Length > MaxBytes)
                return (false, null, $"Ảnh quá lớn (> {MaxBytes / (1024 * 1024)}MB).");

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExts.Contains(ext))
                return (false, null, "Chỉ chấp nhận .jpg, .jpeg, .png, .webp");

            // Đảm bảo thư mục tồn tại
            var root = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var folderPath = Path.Combine(root, UploadFolder.Replace("/", Path.DirectorySeparatorChar.ToString()));
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            // Tạo tên file duy nhất
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
}
