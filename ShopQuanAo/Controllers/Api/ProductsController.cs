using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopQuanAo.Data;
using ShopQuanAo.Models;
using ShopQuanAo.ViewModels;

namespace ShopQuanAo.Controllers.Api
{
    /// <summary>
    /// API sản phẩm: danh sách, chi tiết, bán chạy.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy danh sách sản phẩm (có lọc theo category, tìm kiếm, phân trang).
        /// </summary>
        /// <param name="category">Danh mục (tên category)</param>
        /// <param name="q">Từ khóa tìm kiếm theo tên/mô tả</param>
        /// <param name="page">Trang hiện tại (>=1)</param>
        /// <param name="pageSize">Số sản phẩm / trang (1–60)</param>
        [HttpGet]
        [ProducesResponseType(typeof(ProductListResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<ProductListResponse>> GetList(
            [FromQuery] string? category,
            [FromQuery] string? q,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            // 1️⃣ Base query: chỉ lấy sản phẩm đang bán
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

            // 8️⃣ Trả JSON cho Swagger
            var response = new ProductListResponse
            {
                Items = products,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                Category = category,
                Keyword = q
            };

            return Ok(response);
        }

        /// <summary>
        /// Lấy chi tiết 1 sản phẩm theo ID (kèm giá flash sale nếu có).
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ProductDetailResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductDetailResponse>> GetDetail(int id)
        {
            var product = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id && m.IsAvailable);

            if (product == null)
                return NotFound(new { message = "Không tìm thấy sản phẩm." });

            // Kiểm tra Flash Sale cho sản phẩm này
            var flashSale = await _context.FlashSaleItems
                .FirstOrDefaultAsync(f => f.ProductId == product.Id && f.IsActive && f.EndTime > DateTime.Now);

            var displayPrice = flashSale != null ? flashSale.FlashPrice : product.Price;

            // Sản phẩm liên quan (có thể dùng ở phía client)
            var relatedProducts = await _context.Products
                .AsNoTracking()
                .Where(p => p.Category == product.Category && p.Id != product.Id && p.IsAvailable)
                .OrderByDescending(p => p.Id)
                .Take(4)
                .ToListAsync();

            var dto = new ProductDetailResponse
            {
                Product = product,
                DisplayPrice = displayPrice,
                IsFlashSale = flashSale != null,
                FlashSaleItem = flashSale,
                RelatedProducts = relatedProducts
            };

            return Ok(dto);
        }

        /// <summary>
        /// Danh sách sản phẩm bán chạy nhất (dựa trên OrderItems + Order.Status = Delivered).
        /// </summary>
        /// <param name="top">Số lượng sản phẩm cần lấy (1–50).</param>
        [HttpGet("best-sellers")]
        [ProducesResponseType(typeof(IEnumerable<BestSellerProductViewModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<BestSellerProductViewModel>>> GetBestSellers(
            [FromQuery] int top = 8)
        {
            if (top < 1) top = 4;
            if (top > 50) top = 50;

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

            return Ok(bestSellers);
        }
    }

    /// <summary>Kết quả danh sách sản phẩm có phân trang.</summary>
    public class ProductListResponse
    {
        public List<Product> Items { get; set; } = new();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public string? Category { get; set; }
        public string? Keyword { get; set; }
    }

    /// <summary>Chi tiết sản phẩm + thông tin flash sale + sản phẩm liên quan.</summary>
    public class ProductDetailResponse
    {
        public Product? Product { get; set; }
        public decimal DisplayPrice { get; set; }
        public bool IsFlashSale { get; set; }
        public FlashSaleItem? FlashSaleItem { get; set; }
        public List<Product> RelatedProducts { get; set; } = new();
    }
}
