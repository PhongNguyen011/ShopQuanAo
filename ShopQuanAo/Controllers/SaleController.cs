using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopQuanAo.Data;

namespace ShopQuanAo.Controllers
{
    public class SaleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SaleController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Sale
        public async Task<IActionResult> Index()
        {
            // ✅ Lấy danh sách sản phẩm có IsOnSale = true
            var saleProducts = await _context.Products
                .Where(p => p.IsOnSale && p.IsAvailable)
                .OrderByDescending(p => p.UpdatedDate ?? p.CreatedDate)
                .ToListAsync();

            return View(saleProducts);
        }
    }
}
