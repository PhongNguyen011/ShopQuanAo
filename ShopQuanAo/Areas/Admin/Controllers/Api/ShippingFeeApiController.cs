using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopQuanAo.Data;
using ShopQuanAo.Models;

namespace ShopQuanAo.Areas.Admin.Controllers.Api
{
    /// <summary>
    /// API quản lý phí vận chuyển (ShippingFee) cho Admin.
    /// </summary>
    [Area("Admin")]
    [ApiController]
    [Route("api/admin/shipping-fees")]
    [Authorize(Roles = "Admin")]
    [Produces("application/json")]
    public class ShippingFeeApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private const int DefaultPageSize = 10;

        public ShippingFeeApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        private static string Normalize(string? name)
            => (name ?? string.Empty).Trim().ToLowerInvariant();

        /// <summary>
        /// Lấy danh sách phí ship có phân trang + tìm kiếm theo tỉnh/thành.
        /// </summary>
        /// <param name="search">Từ khoá tên tỉnh/thành (tuỳ chọn).</param>
        /// <param name="page">Trang hiện tại (>=1).</param>
        /// <param name="pageSize">Số bản ghi mỗi trang (mặc định 10).</param>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<ShippingFeeDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<ShippingFeeDto>>> GetAll(
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = DefaultPageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = DefaultPageSize;
            if (pageSize > 100) pageSize = 100;

            var query = _context.ShippingFees.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                query = query.Where(s => s.ProvinceName.Contains(search));
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            if (totalPages == 0) totalPages = 1;
            if (page > totalPages) page = totalPages;

            var items = await query
                .OrderBy(s => s.ProvinceName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new ShippingFeeDto
                {
                    Id = s.Id,
                    ProvinceName = s.ProvinceName,
                    Fee = s.Fee,
                    Description = s.Description,
                    IsActive = s.IsActive,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt
                })
                .ToListAsync();

            var result = new PagedResult<ShippingFeeDto>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            };

            return Ok(result);
        }

        /// <summary>
        /// Lấy chi tiết một phí ship theo Id.
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ShippingFeeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ShippingFeeDto>> GetById(int id)
        {
            var s = await _context.ShippingFees.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (s == null)
                return NotFound(new { message = "Không tìm thấy bản ghi phí ship." });

            var dto = new ShippingFeeDto
            {
                Id = s.Id,
                ProvinceName = s.ProvinceName,
                Fee = s.Fee,
                Description = s.Description,
                IsActive = s.IsActive,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            };

            return Ok(dto);
        }

        /// <summary>
        /// Tạo mới phí ship cho một tỉnh/thành.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ShippingFeeDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ShippingFeeDto>> Create([FromBody] ShippingFeeCreateUpdateDto model)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            if (string.IsNullOrWhiteSpace(model.ProvinceName))
                return BadRequest(new { message = "Tên tỉnh / thành phố không được để trống." });

            var norm = Normalize(model.ProvinceName);
            var exists = await _context.ShippingFees
                .AnyAsync(s => Normalize(s.ProvinceName) == norm);

            if (exists)
            {
                return BadRequest(new { message = "Tỉnh / thành phố này đã có phí ship." });
            }

            var entity = new ShippingFee
            {
                ProvinceName = model.ProvinceName.Trim(),
                Fee = model.Fee,
                Description = model.Description,
                IsActive = model.IsActive,
                CreatedAt = DateTime.Now,
                UpdatedAt = null
            };

            _context.ShippingFees.Add(entity);
            await _context.SaveChangesAsync();

            var dto = new ShippingFeeDto
            {
                Id = entity.Id,
                ProvinceName = entity.ProvinceName,
                Fee = entity.Fee,
                Description = entity.Description,
                IsActive = entity.IsActive,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };

            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        /// <summary>
        /// Cập nhật phí ship theo Id.
        /// </summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ShippingFeeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ShippingFeeDto>> Update(int id, [FromBody] ShippingFeeCreateUpdateDto model)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var entity = await _context.ShippingFees.FindAsync(id);
            if (entity == null)
                return NotFound(new { message = "Không tìm thấy bản ghi phí ship." });

            var norm = Normalize(model.ProvinceName);
            var exists = await _context.ShippingFees
                .AnyAsync(s => Normalize(s.ProvinceName) == norm && s.Id != id);

            if (exists)
            {
                return BadRequest(new { message = "Tỉnh / thành phố này đã có phí ship ở bản ghi khác." });
            }

            entity.ProvinceName = model.ProvinceName.Trim();
            entity.Fee = model.Fee;
            entity.Description = model.Description;
            entity.IsActive = model.IsActive;
            entity.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            var dto = new ShippingFeeDto
            {
                Id = entity.Id,
                ProvinceName = entity.ProvinceName,
                Fee = entity.Fee,
                Description = entity.Description,
                IsActive = entity.IsActive,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };

            return Ok(dto);
        }

        /// <summary>
        /// Xoá một bản ghi phí ship theo Id.
        /// </summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.ShippingFees.FindAsync(id);
            if (entity == null)
                return NotFound(new { message = "Không tìm thấy bản ghi phí ship." });

            _context.ShippingFees.Remove(entity);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Lấy tất cả phí ship đang hoạt động (IsActive = true).
        /// Thích hợp cho phía client khi hiển thị danh sách tỉnh/thành + phí ship.
        /// </summary>
        [HttpGet("active")]
        [AllowAnonymous] // tuỳ bạn, có thể để Admin-only nếu muốn
        [ProducesResponseType(typeof(IEnumerable<ShippingFeeDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ShippingFeeDto>>> GetActive()
        {
            var items = await _context.ShippingFees
                .AsNoTracking()
                .Where(s => s.IsActive)
                .OrderBy(s => s.ProvinceName)
                .Select(s => new ShippingFeeDto
                {
                    Id = s.Id,
                    ProvinceName = s.ProvinceName,
                    Fee = s.Fee,
                    Description = s.Description,
                    IsActive = s.IsActive,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt
                })
                .ToListAsync();

            return Ok(items);
        }
    }

    // ================= DTOs dùng cho Swagger / API =================

    /// <summary>DTO trả về cho ShippingFee.</summary>
    public class ShippingFeeDto
    {
        /// <summary>Id phí ship.</summary>
        public int Id { get; set; }

        /// <summary>Tên tỉnh / thành phố.</summary>
        public string ProvinceName { get; set; } = string.Empty;

        /// <summary>Phí vận chuyển (đồng).</summary>
        public decimal Fee { get; set; }

        /// <summary>Mô tả thêm (tùy chọn).</summary>
        public string? Description { get; set; }

        /// <summary>Còn áp dụng hay không.</summary>
        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>DTO tạo / cập nhật phí ship.</summary>
    public class ShippingFeeCreateUpdateDto
    {
        /// <summary>Tên tỉnh / thành phố.</summary>
        public string ProvinceName { get; set; } = string.Empty;

        /// <summary>Phí vận chuyển (đồng).</summary>
        public decimal Fee { get; set; }

        /// <summary>Mô tả thêm (tùy chọn).</summary>
        public string? Description { get; set; }

        /// <summary>Còn áp dụng hay không.</summary>
        public bool IsActive { get; set; } = true;
    }

    /// <summary>Model phân trang chung.</summary>
    public class PagedResult<T>
    {
        /// <summary>Danh sách item.</summary>
        public List<T> Items { get; set; } = new();

        /// <summary>Trang hiện tại.</summary>
        public int Page { get; set; }

        /// <summary>Số phần tử mỗi trang.</summary>
        public int PageSize { get; set; }

        /// <summary>Tổng số phần tử.</summary>
        public int TotalItems { get; set; }

        /// <summary>Tổng số trang.</summary>
        public int TotalPages { get; set; }
    }
}
