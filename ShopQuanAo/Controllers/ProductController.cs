using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopQuanAo.Data;

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
    }
}
