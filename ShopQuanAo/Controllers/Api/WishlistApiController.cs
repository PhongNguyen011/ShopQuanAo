using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopQuanAo.Data;
using ShopQuanAo.Models;

namespace ShopQuanAo.Controllers
{
    /// <summary>
    /// API quản lý danh sách yêu thích (wishlist) của người dùng.
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class WishlistApiController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public WishlistApiController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        /// <summary>
        /// Lấy danh sách sản phẩm trong wishlist của user hiện tại.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetMyWishlist()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var products = await _db.WishlistItems
                .AsNoTracking()
                .Include(x => x.Product)
                .Where(x => x.ApplicationUserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => x.Product!)
                .ToListAsync();

            return Ok(products);
        }

        /// <summary>
        /// Thêm hoặc bỏ một sản phẩm khỏi wishlist (toggle).
        /// </summary>
        /// <param name="productId">Id sản phẩm</param>
        [HttpPost("toggle/{productId:int}")]
        public async Task<IActionResult> Toggle(int productId)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var existing = await _db.WishlistItems
                .FirstOrDefaultAsync(x => x.ApplicationUserId == userId && x.ProductId == productId);

            bool nowFav;
            if (existing == null)
            {
                _db.WishlistItems.Add(new WishlistItem
                {
                    ApplicationUserId = userId,
                    ProductId = productId,
                    CreatedAt = DateTime.UtcNow
                });
                nowFav = true;
            }
            else
            {
                _db.WishlistItems.Remove(existing);
                nowFav = false;
            }

            await _db.SaveChangesAsync();

            return Ok(new
            {
                ok = true,
                favorited = nowFav,
                productId
            });
        }

        /// <summary>
        /// Xóa 1 sản phẩm ra khỏi wishlist của user hiện tại.
        /// </summary>
        /// <param name="productId">Id sản phẩm</param>
        [HttpDelete("{productId:int}")]
        public async Task<IActionResult> Remove(int productId)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var existing = await _db.WishlistItems
                .FirstOrDefaultAsync(x => x.ApplicationUserId == userId && x.ProductId == productId);

            if (existing != null)
            {
                _db.WishlistItems.Remove(existing);
                await _db.SaveChangesAsync();
            }

            return Ok(new
            {
                ok = true,
                removed = true,
                productId
            });
        }
    }
}
