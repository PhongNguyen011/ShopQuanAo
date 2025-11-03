using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopQuanAo.Data;
using ShopQuanAo.Models;

namespace ShopQuanAo.Controllers
{
    [Authorize]
    public class WishlistController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public WishlistController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User);
            var products = _db.WishlistItems
                .Include(x => x.Product)
                .Where(x => x.ApplicationUserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => x.Product!)
                .ToList();
            return View(products);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(int productId)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var existing = await _db.WishlistItems.FirstOrDefaultAsync(x => x.ApplicationUserId == userId && x.ProductId == productId);
            bool nowFav;
            if (existing == null)
            {
                _db.WishlistItems.Add(new WishlistItem { ApplicationUserId = userId, ProductId = productId });
                nowFav = true;
            }
            else
            {
                _db.WishlistItems.Remove(existing);
                nowFav = false;
            }
            await _db.SaveChangesAsync();
            return Json(new { ok = true, favorited = nowFav });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int productId)
        {
            var userId = _userManager.GetUserId(User);
            var existing = await _db.WishlistItems.FirstOrDefaultAsync(x => x.ApplicationUserId == userId && x.ProductId == productId);
            if (existing != null)
            {
                _db.WishlistItems.Remove(existing);
                await _db.SaveChangesAsync();
            }
            TempData["Info"] = "Đã xoá khỏi yêu thích.";
            return RedirectToAction(nameof(Index));
        }
    }
}


