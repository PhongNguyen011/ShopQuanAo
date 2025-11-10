using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopQuanAo.Data;
using ShopQuanAo.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ShopQuanAo.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ShippingFeeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 10;

        public ShippingFeeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Helper để so sánh tên tỉnh/thành không phân biệt hoa thường, trim khoảng trắng
        private static string Normalize(string? name)
            => (name ?? string.Empty).Trim().ToLower();

        // GET: Admin/ShippingFee
        public async Task<IActionResult> Index(string? search, int page = 1)
        {
            if (page < 1) page = 1;

            var query = _context.ShippingFees.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                query = query.Where(s => s.ProvinceName.Contains(search));
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)PageSize);
            if (totalPages == 0) totalPages = 1;
            if (page > totalPages) page = totalPages;

            var items = await query
                .OrderBy(s => s.ProvinceName)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            ViewBag.Search = search;
            ViewBag.Page = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;

            return View(items);
        }

        // GET: Admin/ShippingFee/Create
        public IActionResult Create()
        {
            // mặc định tick đang hoạt động
            return View(new ShippingFee
            {
                IsActive = true
            });
        }

        // POST: Admin/ShippingFee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ShippingFee model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var norm = Normalize(model.ProvinceName);
            var exists = await _context.ShippingFees
                .AnyAsync(s => Normalize(s.ProvinceName) == norm);

            if (exists)
            {
                ModelState.AddModelError("ProvinceName", "Tỉnh / thành phố này đã có phí ship.");
                return View(model);
            }

            model.CreatedAt = DateTime.Now;
            model.UpdatedAt = null;

            _context.ShippingFees.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/ShippingFee/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _context.ShippingFees.FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        // POST: Admin/ShippingFee/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ShippingFee model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var norm = Normalize(model.ProvinceName);
            var exists = await _context.ShippingFees
                .AnyAsync(s => Normalize(s.ProvinceName) == norm && s.Id != id);

            if (exists)
            {
                ModelState.AddModelError("ProvinceName", "Tỉnh / thành phố này đã có phí ship.");
                return View(model);
            }

            try
            {
                var entity = await _context.ShippingFees.FindAsync(id);
                if (entity == null)
                    return NotFound();

                entity.ProvinceName = model.ProvinceName;
                entity.Fee = model.Fee;
                entity.Description = model.Description;
                entity.IsActive = model.IsActive;
                entity.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.ShippingFees.AnyAsync(e => e.Id == id))
                    return NotFound();

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/ShippingFee/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.ShippingFees
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // POST: Admin/ShippingFee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entity = await _context.ShippingFees.FindAsync(id);
            if (entity != null)
            {
                _context.ShippingFees.Remove(entity);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
