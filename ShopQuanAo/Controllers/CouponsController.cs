using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopQuanAo.Data;
using ShopQuanAo.Models;

namespace ShopQuanAo.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CouponsController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CouponsController(ApplicationDbContext db) => _db = db;

        public async Task<IActionResult> Index(string? q)
        {
            var query = _db.Coupons.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim().ToUpperInvariant();
                query = query.Where(c => c.Code.Contains(q));
            }

            var list = await query.OrderByDescending(c => c.CreatedAt).ToListAsync();
            ViewBag.Q = q;
            return View(list);
        }

        public IActionResult Create() => View(new Coupon
        {
            StartDate = DateTime.UtcNow,
            IsActive = true,
            Scope = CouponScope.All
        });

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Coupon model)
        {
            model.Code = (model.Code ?? "").Trim().ToUpperInvariant();
            if (!ModelState.IsValid) return View(model);

            if (await _db.Coupons.AnyAsync(c => c.Code == model.Code))
            {
                ModelState.AddModelError(nameof(model.Code), "Code đã tồn tại.");
                return View(model);
            }

            model.CreatedAt = DateTime.UtcNow;
            _db.Coupons.Add(model);
            await _db.SaveChangesAsync();
            TempData["ok"] = "Tạo mã thành công.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var c = await _db.Coupons.FindAsync(id);
            return c == null ? NotFound() : View(c);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Coupon model)
        {
            if (id != model.Id) return BadRequest();
            model.Code = (model.Code ?? "").Trim().ToUpperInvariant();
            if (!ModelState.IsValid) return View(model);

            if (await _db.Coupons.AnyAsync(c => c.Code == model.Code && c.Id != id))
            {
                ModelState.AddModelError(nameof(model.Code), "Code đã tồn tại.");
                return View(model);
            }

            model.UpdatedAt = DateTime.UtcNow;
            _db.Entry(model).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            TempData["ok"] = "Cập nhật mã thành công.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var c = await _db.Coupons.FindAsync(id);
            if (c == null) return NotFound();
            return View(c);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var c = await _db.Coupons.FindAsync(id);
            if (c == null) return NotFound();
            _db.Coupons.Remove(c);
            await _db.SaveChangesAsync();
            TempData["ok"] = "Đã xoá mã.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(int id)
        {
            var c = await _db.Coupons.FindAsync(id);
            if (c == null) return NotFound();
            c.IsActive = !c.IsActive;
            c.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
