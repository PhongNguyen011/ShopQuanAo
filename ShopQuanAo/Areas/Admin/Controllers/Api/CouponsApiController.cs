using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopQuanAo.Data;
using ShopQuanAo.Models;

namespace ShopQuanAo.Areas.Admin.Controllers.Api
{
    /// <summary>
    /// API quản lý mã giảm giá (Admin).
    /// </summary>
    [Area("Admin")]
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Produces("application/json")]
    public class CouponsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public CouponsController(ApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>Lấy danh sách tất cả coupon (mới nhất trước).</summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Coupon>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Coupon>>> GetAll([FromQuery] string? q)
        {
            var query = _db.Coupons.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim().ToUpperInvariant();
                query = query.Where(c => c.Code.Contains(q));
            }

            var list = await query
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return Ok(list);
        }

        /// <summary>Lấy chi tiết 1 coupon theo Id.</summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(Coupon), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Coupon>> GetById(int id)
        {
            var c = await _db.Coupons.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (c == null) return NotFound();
            return Ok(c);
        }

        /// <summary>Tạo mới coupon.</summary>
        [HttpPost]
        [ProducesResponseType(typeof(Coupon), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Coupon>> Create([FromBody] Coupon model)
        {
            model.Code = (model.Code ?? "").Trim().ToUpperInvariant();
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (await _db.Coupons.AnyAsync(c => c.Code == model.Code))
            {
                ModelState.AddModelError(nameof(model.Code), "Code đã tồn tại.");
                return BadRequest(ModelState);
            }

            model.CreatedAt = DateTime.UtcNow;
            _db.Coupons.Add(model);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = model.Id }, model);
        }

        /// <summary>Cập nhật coupon.</summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(Coupon), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Coupon>> Update(int id, [FromBody] Coupon model)
        {
            if (id != model.Id) return BadRequest();

            model.Code = (model.Code ?? "").Trim().ToUpperInvariant();
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (await _db.Coupons.AnyAsync(c => c.Code == model.Code && c.Id != id))
            {
                ModelState.AddModelError(nameof(model.Code), "Code đã tồn tại.");
                return BadRequest(ModelState);
            }

            var exist = await _db.Coupons.FindAsync(id);
            if (exist == null) return NotFound();

            // map thủ công hoặc dùng AutoMapper tuỳ bạn, tạm map đơn giản:
            _db.Entry(exist).CurrentValues.SetValues(model);
            exist.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return Ok(exist);
        }

        /// <summary>Xoá coupon.</summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var c = await _db.Coupons.FindAsync(id);
            if (c == null) return NotFound();

            _db.Coupons.Remove(c);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>Đảo trạng thái kích hoạt (IsActive) của coupon.</summary>
        [HttpPost("{id:int}/toggle")]
        [ProducesResponseType(typeof(Coupon), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Coupon>> Toggle(int id)
        {
            var c = await _db.Coupons.FindAsync(id);
            if (c == null) return NotFound();

            c.IsActive = !c.IsActive;
            c.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return Ok(c);
        }
    }
}
