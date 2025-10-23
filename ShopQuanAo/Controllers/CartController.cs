using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopQuanAo.Data;
using ShopQuanAo.Models;
using ShopQuanAo.Services;

namespace ShopQuanAo.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ICartService _cart;

        public CartController(ApplicationDbContext db, ICartService cart)
        {
            _db = db;
            _cart = cart;
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

            ViewBag.StockMap = stockMap;
            ViewBag.Subtotal = items.Sum(x => x.LineTotal);
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
            return RedirectToAction(nameof(Index));
        }
    }
}
