using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopQuanAo.Data;
using ShopQuanAo.Models;
using ShopQuanAo.Services;
using ShopQuanAo.Utils; // <- để dùng SessionExtensions (SetObject/GetObject)
using System.Linq;

namespace ShopQuanAo.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ICartService _cart;
        private readonly ICouponService _coupon; // <- thêm

        public CartController(ApplicationDbContext db, ICartService cart, ICouponService coupon)
        {
            _db = db;
            _cart = cart;
            _coupon = coupon;
        }

        private const string COUPON_KEY = "CART_COUPON";

        public class AppliedCouponInfo
        {
            public string Code { get; set; } = "";
            public decimal Discount { get; set; }
            public string Message { get; set; } = "";
        }

        private AppliedCouponInfo? GetAppliedCoupon()
            => HttpContext.Session.GetObject<AppliedCouponInfo>(COUPON_KEY);

        private void SetAppliedCoupon(AppliedCouponInfo? info)
        {
            if (info == null) HttpContext.Session.Remove(COUPON_KEY);
            else HttpContext.Session.SetObject(COUPON_KEY, info);
        }

        // GET /Cart
        public IActionResult Index()
        {
            var items = _cart.GetCart();

            // Lấy tồn kho cho các sản phẩm trong giỏ
            var ids = items.Select(x => x.ProductId).ToList();
            var stockMap = _db.Products
                              .Where(p => ids.Contains(p.Id))
                              .Select(p => new { p.Id, p.StockQuantity })
                              .ToDictionary(x => x.Id, x => x.StockQuantity);

            var subtotal = items.Sum(x => x.LineTotal);
            var applied = GetAppliedCoupon();
            var discount = applied?.Discount ?? 0m;
            if (discount > subtotal) discount = subtotal;
            var total = subtotal - discount;

            ViewBag.StockMap = stockMap;
            ViewBag.Subtotal = subtotal;
            ViewBag.Discount = discount;
            ViewBag.Total = total;
            ViewBag.AppliedCoupon = applied;
            ViewBag.Message = TempData["CartMessage"];

            return View(items);
        }

        // GET/POST /Cart/Add/5?qty=1  -> thêm xong chuyển vào giỏ, không vượt tồn
        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> Add(int id, int qty = 1)
        {
            if (qty < 1) qty = 1;

            var p = await _db.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (p == null) return NotFound();

            var cart = _cart.GetCart();
            var line = cart.FirstOrDefault(x => x.ProductId == id);

            var stock = p.StockQuantity;
            if (stock <= 0)
            {
                TempData["CartMessage"] = $"Sản phẩm “{p.Name}” đã hết hàng.";
                return RedirectToAction(nameof(Index));
            }

            if (line == null)
            {
                var addQty = Math.Min(qty, stock);
                cart.Add(new CartItem
                {
                    ProductId = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Image = p.ImageUrl,
                    Quantity = addQty
                });
            }
            else
            {
                line.Quantity = Math.Min(line.Quantity + qty, stock);
            }

            _cart.SaveCart(cart);

            // khi thay đổi giỏ, nếu mã đang áp vượt subtotal mới thì vẫn để đó,
            // Index() sẽ tự clamp discount <= subtotal

            TempData["CartMessage"] = $"Đã thêm “{p.Name}” vào giỏ.";
            return RedirectToAction(nameof(Index));
        }

        // 🔥 AJAX: thay đổi số lượng bằng delta (+1, -1)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangeQty(int id, int delta)
        {
            var cart = _cart.GetCart();
            var line = cart.FirstOrDefault(x => x.ProductId == id);
            if (line == null)
            {
                return Json(new { ok = false, error = "Item not found" });
            }

            // Lấy tồn kho hiện tại
            var stock = _db.Products.Where(p => p.Id == id)
                                    .Select(p => p.StockQuantity)
                                    .FirstOrDefault();

            if (stock <= 0)
            {
                // Hết hàng: xoá dòng khỏi giỏ
                cart.RemoveAll(x => x.ProductId == id);
                _cart.SaveCart(cart);
                return Json(new
                {
                    ok = true,
                    removed = true,
                    subtotal = cart.Sum(x => x.LineTotal)
                });
            }

            var newQty = line.Quantity + delta;
            if (newQty < 1) newQty = 1;
            if (newQty > stock) newQty = stock;

            line.Quantity = newQty;
            _cart.SaveCart(cart);

            // Không cần xử lý lại coupon ở đây, Index() sẽ clamp

            return Json(new
            {
                ok = true,
                qty = line.Quantity,
                lineTotal = line.LineTotal,
                subtotal = cart.Sum(x => x.LineTotal),
                stock = stock,
                maxed = line.Quantity >= stock,
                mined = line.Quantity <= 1
            });
        }

        // (Giữ nguyên Remove/Clear nếu bạn đã có)
        [HttpPost]
        public IActionResult Remove(int id)
        {
            var cart = _cart.GetCart();
            cart.RemoveAll(x => x.ProductId == id);
            _cart.SaveCart(cart);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Clear()
        {
            _cart.SaveCart(new List<CartItem>());
            SetAppliedCoupon(null); // xoá luôn mã nếu dọn giỏ
            return RedirectToAction(nameof(Index));
        }

        // ================== COUPON ==================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApplyCoupon(string code)
        {
            var cart = _cart.GetCart();
            if (cart.Count == 0)
            {
                SetAppliedCoupon(null);
                TempData["CartMessage"] = "Giỏ hàng trống.";
                return RedirectToAction(nameof(Index));
            }

            var result = await _coupon.TryApplyAsync(code, cart);
            if (!result.ok)
            {
                SetAppliedCoupon(null);
                TempData["CartMessage"] = result.message;
            }
            else
            {
                SetAppliedCoupon(new AppliedCouponInfo
                {
                    Code = result.coupon!.Code,
                    Discount = result.discount,
                    Message = result.message
                });
                TempData["CartMessage"] = result.message;
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveCoupon()
        {
            SetAppliedCoupon(null);
            TempData["CartMessage"] = "Đã bỏ mã giảm giá.";
            return RedirectToAction(nameof(Index));
        }
    }
}
