using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopQuanAo.Data;
using ShopQuanAo.ViewModels;

namespace ShopQuanAo.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Product
        public async Task<IActionResult> Index(string? category, string? q, int page = 1, int pageSize = 10)
        {
            // 1️⃣ Base query
            var query = _context.Products
                .AsNoTracking()
                .Where(p => p.IsAvailable);

            // 2️⃣ Lọc theo danh mục
            if (!string.IsNullOrWhiteSpace(category))
            {
                var cat = category.Trim();
                query = query.Where(p => p.Category != null && p.Category.ToLower() == cat.ToLower());
            }

            // 3️⃣ Tìm kiếm
            if (!string.IsNullOrWhiteSpace(q))
            {
                var kw = q.Trim();
                var like = $"%{kw}%";
                query = query.Where(p =>
                    EF.Functions.Like(p.Name, like) ||
                    (p.Description != null && EF.Functions.Like(p.Description, like))
                );
            }

            // 4️⃣ Tổng số sản phẩm
            var totalItems = await query.CountAsync();

            // 5️⃣ Giới hạn pageSize
            pageSize = pageSize switch
            {
                < 1 => 5,
                > 60 => 60,
                _ => pageSize
            };

            // 6️⃣ Tính phân trang
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            page = Math.Clamp(page, 1, Math.Max(1, totalPages));

            // 7️⃣ Lấy sản phẩm trang hiện tại
            var products = await query
                .OrderByDescending(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // 8️⃣ Truyền dữ liệu sang View
            ViewBag.SelectedCategory = category;
            ViewBag.Keyword = q;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.TotalPages = totalPages;

            return View(products);
        }

        // GET: /Product/Detail/5
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
                return NotFound();

            var product = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id && m.IsAvailable);

            if (product == null)
                return NotFound();

            // Kiểm tra Flash Sale cho sản phẩm này
            var flashSale = await _context.FlashSaleItems
                .FirstOrDefaultAsync(f => f.ProductId == product.Id && f.IsActive && f.EndTime > DateTime.Now);

            var displayPrice = flashSale != null ? flashSale.FlashPrice : product.Price;
            ViewBag.DisplayPrice = displayPrice;
            ViewBag.IsFlashSale = flashSale != null;
            ViewBag.FlashSaleItem = flashSale;

            // Sản phẩm liên quan
            var relatedProducts = await _context.Products
                .AsNoTracking()
                .Where(p => p.Category == product.Category && p.Id != product.Id && p.IsAvailable)
                .OrderByDescending(p => p.Id)
                .Take(4)
                .ToListAsync();

            ViewBag.RelatedProducts = relatedProducts;
            return View(product);
        }

        // 🔥 GET: /Product/BestSellers
        // Trang hiển thị danh sách sản phẩm bán chạy cho user
        public async Task<IActionResult> BestSellers(int top = 8)
        {
            if (top < 1) top = 4;
            if (top > 50) top = 50;

            // Tính tổng số lượng đã bán cho từng sản phẩm
            // Dựa trên OrderItems + Orders.Status = Delivered
            var bestQuery =
                from oi in _context.OrderItems
                join o in _context.Orders on oi.OrderId equals o.Id
                join p in _context.Products on oi.ProductName equals p.Name
                where o.Status == "Delivered" && p.IsAvailable
                group new { oi, p } by new
                {
                    p.Id,
                    p.Name,
                    p.ImageUrl,
                    p.Price,
                    p.OldPrice,
                    p.Category,
                    p.IsAvailable,
                    p.IsFeatured,
                    p.IsOnSale
                }
                into g
                orderby g.Sum(x => x.oi.Quantity) descending
                select new BestSellerProductViewModel
                {
                    ProductId = g.Key.Id,
                    Name = g.Key.Name,
                    ImageUrl = g.Key.ImageUrl,
                    Price = g.Key.Price,
                    OldPrice = g.Key.OldPrice,
                    Category = g.Key.Category,
                    IsAvailable = g.Key.IsAvailable,
                    IsFeatured = g.Key.IsFeatured,
                    IsOnSale = g.Key.IsOnSale,
                    SoldQuantity = g.Sum(x => x.oi.Quantity),
                    Revenue = g.Sum(x => x.oi.LineTotal)
                };

            var bestSellers = await bestQuery
                .Take(top)
                .ToListAsync();

            return View(bestSellers);
        }
    }
}
