using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopQuanAo.Models;
using System.IO;
using System.Linq;
using ShopQuanAo.Data;

namespace ShopQuanAo.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;
        private readonly ApplicationDbContext _db;

        public ProfileController(UserManager<ApplicationUser> userManager, IWebHostEnvironment env, ApplicationDbContext db)
        {
            _userManager = userManager;
            _env = env;
            _db = db;
        }

        // GET: /Profile
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");
            return View(user);
        }

        // GET: /Profile/Edit
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");
            return View(user);
        }

        // POST: /Profile/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ApplicationUser model, IFormFile? AvatarFile)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            // cập nhật thông tin text
            user.FullName = model.FullName?.Trim();
            user.Address = model.Address?.Trim();
            user.PhoneNumber = model.PhoneNumber?.Trim();
            user.DateOfBirth = model.DateOfBirth;
            user.UpdatedDate = DateTime.Now;

            // xử lý ảnh nếu có upload
            if (AvatarFile != null && AvatarFile.Length > 0)
            {
                var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var ext = Path.GetExtension(AvatarFile.FileName).ToLowerInvariant();

                if (!allowed.Contains(ext))
                {
                    ModelState.AddModelError("AvatarFileName", "Chỉ chấp nhận JPG/PNG/GIF/WEBP.");
                    return View(model);
                }
                if (AvatarFile.Length > 2 * 1024 * 1024)
                {
                    ModelState.AddModelError("AvatarFileName", "Ảnh vượt quá 2MB.");
                    return View(model);
                }

                var folder = Path.Combine(_env.WebRootPath, "images", "account");
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                // Xoá ảnh cũ nếu có
                if (!string.IsNullOrEmpty(user.AvatarFileName))
                {
                    var oldPath = Path.Combine(folder, user.AvatarFileName);
                    if (System.IO.File.Exists(oldPath))
                    {
                        try { System.IO.File.Delete(oldPath); } catch { /* ignore */ }
                    }
                }

                // Lưu ảnh mới
                var newName = $"{Guid.NewGuid()}{ext}";
                var savePath = Path.Combine(folder, newName);
                using (var fs = new FileStream(savePath, FileMode.Create))
                {
                    await AvatarFile.CopyToAsync(fs);
                }
                user.AvatarFileName = newName;
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var e in result.Errors) ModelState.AddModelError("", e.Description);
                return View(model);
            }

            TempData["Success"] = "Cập nhật hồ sơ thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> MyOrders()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");
            var orders = _db.Orders
                .Where(x => x.ApplicationUserId == user.Id)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();
            return View(orders);
        }
    }
}
