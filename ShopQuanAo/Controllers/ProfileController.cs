using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopQuanAo.Data;
using ShopQuanAo.Models;
using ShopQuanAo.Models.ViewModels;
using System.IO;
using System.Linq;

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

        // GET: /Profile/OrderDetails/OrderCode...
        [HttpGet]
        public async Task<IActionResult> OrderDetail(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            if (string.IsNullOrWhiteSpace(id))
            {
                TempData["Error"] = "Mã đơn hàng không hợp lệ.";
                return RedirectToAction(nameof(MyOrders));
            }

            // Tìm đơn theo OrderCode + đúng user
            var order = await _db.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o =>
                    o.OrderCode == id &&
                    o.ApplicationUserId == user.Id);

            if (order == null)
            {
                TempData["Error"] = "Không tìm thấy đơn hàng.";
                return RedirectToAction(nameof(MyOrders));
            }

            return View(order);
        }


        // GET: /Profile/ChangePassword
        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            // XÓA mọi lỗi cũ để lần đầu load form không bị đỏ
            ModelState.Clear();

            return View(new ShopQuanAo.Models.ViewModels.ChangePasswordViewModel());
        }

        // POST: /Profile/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ShopQuanAo.Models.ViewModels.ChangePasswordViewModel model)
        {
            // trường hợp form còn trống hoặc sai => show lỗi
            if (!ModelState.IsValid)
            {
                // KHÔNG Clear ModelState ở đây, vì ta muốn hiện lỗi sau khi bấm submit
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Error"] = "Không tìm thấy tài khoản.";
                return RedirectToAction(nameof(ChangePassword));
            }

            var result = await _userManager.ChangePasswordAsync(
                user,
                model.OldPassword,
                model.NewPassword
            );

            if (!result.Succeeded)
            {
                foreach (var e in result.Errors)
                {
                    ModelState.AddModelError("", e.Description);
                }

                return View(model); // vẫn trả lại view với lỗi
            }

            user.UpdatedDate = DateTime.Now;
            await _userManager.UpdateAsync(user);

            TempData["Success"] = "Đổi mật khẩu thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}
