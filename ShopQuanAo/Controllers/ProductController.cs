using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopQuanAo.Data;

namespace ShopQuanAo.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Shop
        public async Task<IActionResult> Index(string? category, string? q)
        {
            var query = _context.Products.AsQueryable();
            query = query.Where(p => p.IsAvailable);

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(p => p.Category == category);
            }

            if (!string.IsNullOrWhiteSpace(q))
            {
                var kw = q.Trim();
                query = query.Where(p => p.Name.Contains(kw) || (p.Description != null && p.Description.Contains(kw)));
            }

            var products = await query.ToListAsync();

            ViewBag.SelectedCategory = category;
            ViewBag.Keyword = q;
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

