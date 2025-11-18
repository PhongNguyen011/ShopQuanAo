using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopQuanAo.Data;
using ShopQuanAo.Models;

namespace ShopQuanAo.Areas.Admin.Controllers.Api
{
    /// <summary>
    /// API quản lý sản phẩm dành cho Admin.
    /// </summary>
    [Area("Admin")]
    [ApiController]
    [Route("api/admin/[controller]")]
    [Produces("application/json")]
    [Authorize(Roles = "Admin")]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public ProductsController(ApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Lấy danh sách sản phẩm (có tìm kiếm, lọc category, phân trang) cho Admin.
        /// </summary>
        /// <param name="category">Danh mục (chuỗi Category trong Product)</param>
        /// <param name="q">Từ khóa tìm kiếm theo tên/mô tả</param>
        /// <param name="page">Trang hiện tại (>=1)</param>
        /// <param name="pageSize">Số sản phẩm / trang (1–100)</param>
        [HttpGet]
        [ProducesResponseType(typeof(AdminProductListResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<AdminProductListResponse>> GetList(
            [FromQuery] string? category,
            [FromQuery] string? q,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var query = _db.Products.AsNoTracking().AsQueryable();

            // Lọc category
            if (!string.IsNullOrWhiteSpace(category))
            {
                var cat = category.Trim();
                query = query.Where(p => p.Category != null &&
                                         p.Category.ToLower() == cat.ToLower());
            }

            // Tìm kiếm
            if (!string.IsNullOrWhiteSpace(q))
            {
                var kw = q.Trim();
                var like = $"%{kw}%";
                query = query.Where(p =>
                    EF.Functions.Like(p.Name, like) ||
                    (p.Description != null && EF.Functions.Like(p.Description, like))
                );
            }

            var totalItems = await query.CountAsync();

            // Giới hạn pageSize
            pageSize = pageSize switch
            {
                < 1 => 10,
                > 100 => 100,
                _ => pageSize
            };

            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            page = Math.Clamp(page, 1, Math.Max(1, totalPages));

            var items = await query
                .OrderByDescending(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var res = new AdminProductListResponse
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                Category = category,
                Keyword = q
            };

            return Ok(res);
        }

        /// <summary>Lấy chi tiết sản phẩm theo Id.</summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Product>> GetById(int id)
        {
            var p = await _db.Products.FindAsync(id);
            if (p == null) return NotFound(new { message = "Không tìm thấy sản phẩm." });
            return Ok(p);
        }

        /// <summary>Tạo mới sản phẩm.</summary>
        [HttpPost]
        [ProducesResponseType(typeof(Product), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Product>> Create([FromBody] AdminProductCreateUpdateDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var p = new Product
            {
                Name = model.Name.Trim(),
                Description = model.Description,
                Category = model.Category,
                Price = model.Price,
                OldPrice = model.OldPrice,
                ImageUrl = model.ImageUrl,
                StockQuantity = model.StockQuantity,
                IsAvailable = model.IsAvailable,
                IsOnSale = model.IsOnSale,
                IsFeatured = model.IsFeatured,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };

            _db.Products.Add(p);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = p.Id }, p);
        }

        /// <summary>Cập nhật thông tin sản phẩm.</summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Product>> Update(int id, [FromBody] AdminProductCreateUpdateDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var p = await _db.Products.FindAsync(id);
            if (p == null) return NotFound(new { message = "Không tìm thấy sản phẩm." });

            p.Name = model.Name.Trim();
            p.Description = model.Description;
            p.Category = model.Category;
            p.Price = model.Price;
            p.OldPrice = model.OldPrice;
            p.ImageUrl = model.ImageUrl;
            p.StockQuantity = model.StockQuantity;
            p.IsAvailable = model.IsAvailable;
            p.IsOnSale = model.IsOnSale;
            p.IsFeatured = model.IsFeatured;
            p.UpdatedDate = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return Ok(p);
        }

        /// <summary>Xoá sản phẩm (hard delete).</summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var p = await _db.Products.FindAsync(id);
            if (p == null) return NotFound(new { message = "Không tìm thấy sản phẩm." });

            _db.Products.Remove(p);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>Bật / tắt trạng thái bán (IsAvailable).</summary>
        [HttpPost("{id:int}/toggle-available")]
        [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Product>> ToggleAvailable(int id)
        {
            var p = await _db.Products.FindAsync(id);
            if (p == null) return NotFound(new { message = "Không tìm thấy sản phẩm." });

            p.IsAvailable = !p.IsAvailable;
            p.UpdatedDate = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return Ok(p);
        }
    }

    /// <summary>DTO dùng để trả list sản phẩm phía Admin.</summary>
    public class AdminProductListResponse
    {
        public List<Product> Items { get; set; } = new();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public string? Category { get; set; }
        public string? Keyword { get; set; }
    }

    /// <summary>DTO tạo / cập nhật sản phẩm trong Admin.</summary>
    public class AdminProductCreateUpdateDto
    {
        /// <summary>Tên sản phẩm.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Mô tả.</summary>
        public string? Description { get; set; }

        /// <summary>Danh mục (chuỗi).</summary>
        public string? Category { get; set; }

        /// <summary>Giá hiện tại.</summary>
        public decimal Price { get; set; }

        /// <summary>Giá cũ (nếu có).</summary>
        public decimal? OldPrice { get; set; }

        /// <summary>Đường dẫn ảnh (ImageUrl).</summary>
        public string? ImageUrl { get; set; }

        /// <summary>Tồn kho.</summary>
        public int StockQuantity { get; set; }

        /// <summary>Đang bán hay không.</summary>
        public bool IsAvailable { get; set; } = true;

        /// <summary>Đang khuyến mãi.</summary>
        public bool IsOnSale { get; set; }

        /// <summary>Gắn cờ nổi bật.</summary>
        public bool IsFeatured { get; set; }
    }
}
