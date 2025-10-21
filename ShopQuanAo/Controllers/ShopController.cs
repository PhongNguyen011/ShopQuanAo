using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopQuanAo.Data;

namespace ShopQuanAo.Controllers
{
    public class ShopController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShopController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Shop
        public async Task<IActionResult> Index(string? category)
        {
            var products = await _context.Products
                .Where(p => p.IsAvailable)
                .ToListAsync();

            if (!string.IsNullOrEmpty(category))
            {
                products = products.Where(p => p.Category == category).ToList();
            }

            ViewBag.SelectedCategory = category;
            return View(products);
        }

        // GET: Shop/Detail/5
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id && m.IsAvailable);

            if (product == null)
            {
                return NotFound();
            }

            // Get related products (same category)
            var relatedProducts = await _context.Products
                .Where(p => p.Category == product.Category && p.Id != product.Id && p.IsAvailable)
                .Take(4)
                .ToListAsync();

            ViewBag.RelatedProducts = relatedProducts;

            return View(product);
        }
    }
}

