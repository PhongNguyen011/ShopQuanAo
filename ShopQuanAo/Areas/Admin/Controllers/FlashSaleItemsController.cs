using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShopQuanAo.Data;
using ShopQuanAo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopQuanAo.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FlashSaleItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FlashSaleItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ----------------------------------------
        // Helper: dropdown sản phẩm
        // ----------------------------------------
        private async Task<List<SelectListItem>> BuildProductSelectListAsync(int? selectedId = null)
        {
            var products = await _context
                .Products
                .Where(p => p.IsAvailable && p.StockQuantity > 0)
                .OrderBy(p => p.Name)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Price,
                    p.StockQuantity
                })
                .ToListAsync();

            return products
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = $"{p.Name} ({p.Price.ToString("N0")}đ / tồn:{p.StockQuantity})",
                    Selected = (selectedId.HasValue && p.Id == selectedId.Value)
                })
                .ToList();
        }

        // ----------------------------------------
        // GET: Admin/FlashSaleItems
        // ----------------------------------------
        public async Task<IActionResult> Index()
        {
            // dọn bản ghi đã hết hạn
            var expired = await _context.FlashSaleItems
                .Where(f => f.EndTime < DateTime.Now)
                .ToListAsync();

            if (expired.Any())
            {
                _context.FlashSaleItems.RemoveRange(expired);
                await _context.SaveChangesAsync();
            }

            // danh sách hiện tại
            var list = await _context.FlashSaleItems
                .Include(f => f.Product)
                .OrderByDescending(f => f.IsActive)
                .ThenBy(f => f.EndTime)
                .ToListAsync();

            ViewData["Title"] = "Flash Sale";
            ViewData["Card"] = "Danh sách";

            return View(list);
        }

        // ----------------------------------------
        // GET: Create
        // ----------------------------------------
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewData["Title"] = "Flash Sale";
            ViewData["Card"] = "Thêm Flash Sale";

            ViewBag.ProductList = await BuildProductSelectListAsync(null);

            var vm = new FlashSaleItem
            {
                EndTime = DateTime.Now.AddDays(1),
                IsActive = true
            };

            return View(vm);
        }

        // ----------------------------------------
        // POST: Create
        // ----------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FlashSaleItem formData)
        {
            ViewData["Title"] = "Flash Sale";
            ViewData["Card"] = "Thêm Flash Sale";

            // lấy sản phẩm
            var product = await _context.Products.FindAsync(formData.ProductId);

            // VALIDATION
            if (product == null)
            {
                ModelState.AddModelError("ProductId", "Sản phẩm không tồn tại.");
            }

            if (formData.EndTime <= DateTime.Now)
            {
                ModelState.AddModelError("EndTime", "Thời gian kết thúc phải lớn hơn hiện tại.");
            }

            // OPTIONAL: giới hạn tối đa 7 ngày
            if (formData.EndTime > DateTime.Now.AddDays(7))
            {
                ModelState.AddModelError("EndTime", "Flash Sale tối đa 7 ngày.");
            }

            // FlashPrice phải chia hết cho 1000
            if (((long)formData.FlashPrice % 1000) != 0)
            {
                ModelState.AddModelError("FlashPrice", "Giá phải bội số của 1000.");
            }

            // FlashPrice phải nhỏ hơn giá gốc
            if (product != null && formData.FlashPrice >= product.Price)
            {
                ModelState.AddModelError("FlashPrice", "Giá Flash phải thấp hơn giá gốc của sản phẩm.");
            }

            // Không cho tạo thêm nếu đã có FlashSale active cho sản phẩm này
            var overlapping = await _context.FlashSaleItems
                .Where(f => f.ProductId == formData.ProductId
                            && f.EndTime > DateTime.Now)
                .AnyAsync();
            if (overlapping)
            {
                ModelState.AddModelError("ProductId", "Sản phẩm này đã có Flash Sale đang chạy.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ProductList = await BuildProductSelectListAsync(formData.ProductId);
                TempData["Error"] = "Không thể lưu Flash Sale. Kiểm tra các trường được đánh dấu.";
                return View(formData);
            }

            // HỢP LỆ -> tạo mới
            formData.CreatedAt = DateTime.Now;
           

            _context.FlashSaleItems.Add(formData);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã tạo Flash Sale.";
            return RedirectToAction(nameof(Index));
        }

        // ----------------------------------------
        // GET: Edit
        // ----------------------------------------
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _context.FlashSaleItems
                .Include(f => f.Product)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (item == null)
                return NotFound();

            ViewData["Title"] = "Flash Sale";
            ViewData["Card"] = "Chỉnh sửa Flash Sale";

            ViewBag.ProductList = await BuildProductSelectListAsync(item.ProductId);

            return View(item);
        }

        // ----------------------------------------
        // POST: Edit
        // ----------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FlashSaleItem formData)
        {
            if (id != formData.Id)
                return NotFound();

            ViewData["Title"] = "Flash Sale";
            ViewData["Card"] = "Chỉnh sửa Flash Sale";

            // lấy record gốc
            var existing = await _context.FlashSaleItems
                .FirstOrDefaultAsync(f => f.Id == id);

            if (existing == null)
                return NotFound();

            // lấy sản phẩm
            var product = await _context.Products.FindAsync(formData.ProductId);

            // VALIDATION (giống Create)
            if (product == null)
            {
                ModelState.AddModelError("ProductId", "Sản phẩm không tồn tại.");
            }

            if (formData.EndTime <= DateTime.Now)
            {
                ModelState.AddModelError("EndTime", "Thời gian kết thúc phải lớn hơn hiện tại.");
            }

            if (formData.EndTime > DateTime.Now.AddDays(7))
            {
                ModelState.AddModelError("EndTime", "Flash Sale tối đa 7 ngày.");
            }

            if (((long)formData.FlashPrice % 1000) != 0)
            {
                ModelState.AddModelError("FlashPrice", "Giá phải bội số của 1000.");
            }

            if (product != null && formData.FlashPrice >= product.Price)
            {
                ModelState.AddModelError("FlashPrice", "Giá Flash phải thấp hơn giá gốc của sản phẩm.");
            }

            var overlapping = await _context.FlashSaleItems
                .Where(f => f.ProductId == formData.ProductId
                            && f.Id != formData.Id
                            && f.EndTime > DateTime.Now)
                .AnyAsync();
            if (overlapping)
            {
                ModelState.AddModelError("ProductId", "Sản phẩm này đã có Flash Sale đang chạy.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ProductList = await BuildProductSelectListAsync(formData.ProductId);
                TempData["Error"] = "Không thể cập nhật Flash Sale.";
                return View(formData);
            }

            // GÁN LẠI GIÁ TRỊ (luôn ghi, kể cả không đổi gì)
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
                TempData["Success"] = "Đã cập nhật Flash Sale.";
            }
            catch (DbUpdateConcurrencyException)
            {
                bool stillExists = await _context.FlashSaleItems.AnyAsync(f => f.Id == id);
                if (!stillExists)
                    return NotFound();

                return Problem("Lỗi khi cập nhật DB.");
            }

            return RedirectToAction(nameof(Index));
        }

        // ----------------------------------------
        // GET: Delete
        // ----------------------------------------
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.FlashSaleItems
                .Include(f => f.Product)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (item == null)
                return NotFound();

            ViewData["Title"] = "Flash Sale";
            ViewData["Card"] = "Xóa Flash Sale";

            return View(item);
        }

        // ----------------------------------------
        // POST: DeleteConfirmed
        // ----------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.FlashSaleItems.FindAsync(id);
            if (item != null)
            {
                _context.FlashSaleItems.Remove(item);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Đã xóa Flash Sale.";
            }

            return RedirectToAction(nameof(Index));
        }

        // ----------------------------------------
        // POST: Toggle
        // ----------------------------------------
        [HttpPost]
        public async Task<IActionResult> Toggle(int id)
        {
            var item = await _context.FlashSaleItems.FindAsync(id);
            if (item == null)
                return NotFound();

            // nếu đã hết hạn thì không cho bật
            if (item.EndTime <= DateTime.Now)
            {
                item.IsActive = false;
              
                await _context.SaveChangesAsync();
                TempData["Error"] = "Flash Sale này đã hết hạn, không thể bật lại.";
                return RedirectToAction(nameof(Index));
            }

            item.IsActive = !item.IsActive;
        

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Đã {(item.IsActive ? "bật" : "tắt")} Flash Sale.";
            return RedirectToAction(nameof(Index));
        }
    }
}
