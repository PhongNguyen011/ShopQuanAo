using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopQuanAo.Data;
using ShopQuanAo.Models;

namespace ShopQuanAo.Areas.Admin.Controllers.Api
{
    /// <summary>
    /// API quản lý Flash Sale (FlashSaleItems) dành cho Admin.
    /// </summary>
    [Area("Admin")]
    [ApiController]
    [Route("api/admin/[controller]")]
    [Authorize(Roles = "Admin")]
    [Produces("application/json")]
    public class FlashSaleItemsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FlashSaleItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Danh sách Flash Sale hiện tại (tự dọn bản ghi đã hết hạn).
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<FlashSaleItem>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<FlashSaleItem>>> GetAll()
        {
            // Dọn bản ghi đã hết hạn
            var expired = await _context.FlashSaleItems
                .Where(f => f.EndTime < DateTime.Now)
                .ToListAsync();

            if (expired.Any())
            {
                _context.FlashSaleItems.RemoveRange(expired);
                await _context.SaveChangesAsync();
            }

            var list = await _context.FlashSaleItems
                .Include(f => f.Product)
                .OrderByDescending(f => f.IsActive)
                .ThenBy(f => f.EndTime)
                .ToListAsync();

            return Ok(list);
        }

        /// <summary>
        /// Lấy chi tiết 1 Flash Sale theo Id.
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(FlashSaleItem), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FlashSaleItem>> GetById(int id)
        {
            var item = await _context.FlashSaleItems
                .Include(f => f.Product)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (item == null) return NotFound(new { message = "Không tìm thấy Flash Sale." });

            return Ok(item);
        }

        /// <summary>
        /// Tạo mới Flash Sale.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(FlashSaleItem), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<FlashSaleItem>> Create([FromBody] FlashSaleItemDto formData)
        {
            // Lấy sản phẩm
            var product = await _context.Products.FindAsync(formData.ProductId);

            // VALIDATION
            if (product == null)
            {
                ModelState.AddModelError(nameof(formData.ProductId), "Sản phẩm không tồn tại.");
            }

            if (formData.EndTime <= DateTime.Now)
            {
                ModelState.AddModelError(nameof(formData.EndTime), "Thời gian kết thúc phải lớn hơn hiện tại.");
            }

            // Giới hạn tối đa 7 ngày
            if (formData.EndTime > DateTime.Now.AddDays(7))
            {
                ModelState.AddModelError(nameof(formData.EndTime), "Flash Sale tối đa 7 ngày.");
            }

            // FlashPrice phải chia hết cho 1000
            if (((long)formData.FlashPrice % 1000) != 0)
            {
                ModelState.AddModelError(nameof(formData.FlashPrice), "Giá phải bội số của 1000.");
            }

            // FlashPrice phải nhỏ hơn giá gốc
            if (product != null && formData.FlashPrice >= product.Price)
            {
                ModelState.AddModelError(nameof(formData.FlashPrice), "Giá Flash phải thấp hơn giá gốc của sản phẩm.");
            }

            // Không cho tạo thêm nếu đã có FlashSale active cho sản phẩm này
            var overlapping = await _context.FlashSaleItems
                .Where(f => f.ProductId == formData.ProductId
                            && f.EndTime > DateTime.Now)
                .AnyAsync();
            if (overlapping)
            {
                ModelState.AddModelError(nameof(formData.ProductId), "Sản phẩm này đã có Flash Sale đang chạy.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var item = new FlashSaleItem
            {
                ProductId = formData.ProductId,
                FlashPrice = formData.FlashPrice,
                EndTime = formData.EndTime,
                IsActive = formData.IsActive,
                CreatedAt = DateTime.Now
            };

            _context.FlashSaleItems.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }

        /// <summary>
        /// Cập nhật 1 Flash Sale.
        /// </summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(FlashSaleItem), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FlashSaleItem>> Update(int id, [FromBody] FlashSaleItemDto formData)
        {
            if (id != formData.Id)
            {
                return BadRequest(new { message = "Id không khớp." });
            }

            var existing = await _context.FlashSaleItems
                .FirstOrDefaultAsync(f => f.Id == id);

            if (existing == null)
                return NotFound(new { message = "Không tìm thấy Flash Sale." });

            // Lấy sản phẩm
            var product = await _context.Products.FindAsync(formData.ProductId);

            // VALIDATION (giống Create)
            if (product == null)
            {
                ModelState.AddModelError(nameof(formData.ProductId), "Sản phẩm không tồn tại.");
            }

            if (formData.EndTime <= DateTime.Now)
            {
                ModelState.AddModelError(nameof(formData.EndTime), "Thời gian kết thúc phải lớn hơn hiện tại.");
            }

            if (formData.EndTime > DateTime.Now.AddDays(7))
            {
                ModelState.AddModelError(nameof(formData.EndTime), "Flash Sale tối đa 7 ngày.");
            }

            if (((long)formData.FlashPrice % 1000) != 0)
            {
                ModelState.AddModelError(nameof(formData.FlashPrice), "Giá phải bội số của 1000.");
            }

            if (product != null && formData.FlashPrice >= product.Price)
            {
                ModelState.AddModelError(nameof(formData.FlashPrice), "Giá Flash phải thấp hơn giá gốc của sản phẩm.");
            }

            var overlapping = await _context.FlashSaleItems
                .Where(f => f.ProductId == formData.ProductId
                            && f.Id != formData.Id
                            && f.EndTime > DateTime.Now)
                .AnyAsync();
            if (overlapping)
            {
                ModelState.AddModelError(nameof(formData.ProductId), "Sản phẩm này đã có Flash Sale đang chạy.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Gán lại giá trị
            existing.ProductId = formData.ProductId;
            existing.FlashPrice = formData.FlashPrice;
            existing.EndTime = formData.EndTime;
            existing.IsActive = formData.IsActive;

            // auto tắt nếu đã quá hạn
            if (existing.EndTime <= DateTime.Now)
            {
                existing.IsActive = false;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                bool stillExists = await _context.FlashSaleItems.AnyAsync(f => f.Id == id);
                if (!stillExists)
                    return NotFound(new { message = "Flash Sale đã bị xoá trong quá trình cập nhật." });

                return Problem("Lỗi khi cập nhật DB.");
            }

            return Ok(existing);
        }

        /// <summary>
        /// Xoá 1 Flash Sale.
        /// </summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.FlashSaleItems.FindAsync(id);
            if (item == null)
                return NotFound(new { message = "Không tìm thấy Flash Sale." });

            _context.FlashSaleItems.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Bật / tắt trạng thái Flash Sale (IsActive).
        /// </summary>
        [HttpPost("{id:int}/toggle")]
        [ProducesResponseType(typeof(FlashSaleItem), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FlashSaleItem>> Toggle(int id)
        {
            var item = await _context.FlashSaleItems.FindAsync(id);
            if (item == null)
                return NotFound(new { message = "Không tìm thấy Flash Sale." });

            // nếu đã hết hạn thì không cho bật
            if (item.EndTime <= DateTime.Now)
            {
                item.IsActive = false;
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    message = "Flash Sale này đã hết hạn, không thể bật lại.",
                    item
                });
            }

            item.IsActive = !item.IsActive;
            await _context.SaveChangesAsync();

            return Ok(item);
        }
    }

    /// <summary>
    /// DTO tạo/cập nhật Flash Sale (Admin API).
    /// </summary>
    public class FlashSaleItemDto
    {
        /// <summary>Id Flash Sale (update dùng, create có thể để 0).</summary>
        public int Id { get; set; }

        /// <summary>Id sản phẩm áp dụng Flash Sale.</summary>
        public int ProductId { get; set; }

        /// <summary>Giá Flash (bội số 1000, nhỏ hơn giá gốc).</summary>
        public decimal FlashPrice { get; set; }

        /// <summary>Thời gian kết thúc Flash Sale.</summary>
        public DateTime EndTime { get; set; }

        /// <summary>Trạng thái active.</summary>
        public bool IsActive { get; set; } = true;
    }
}
