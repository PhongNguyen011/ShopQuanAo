using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopQuanAo.Data;

namespace ShopQuanAo.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PostsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Posts
        public async Task<IActionResult> Index()
        {
            var posts = await _context.Posts
                .AsNoTracking()
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
            return View(posts);
        }

        // GET: /Posts/Details/slug
        public async Task<IActionResult> Details(string slug)
        {
            if (string.IsNullOrEmpty(slug)) return NotFound();

            var post = await _context.Posts
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Slug == slug);

            if (post == null) return NotFound();

            // Bài viết khác để hiển thị bên dưới hoặc sidebar
            ViewBag.Related = await _context.Posts
                .AsNoTracking()
                .Where(p => p.Id != post.Id)
                .OrderByDescending(p => p.CreatedAt)
                .Take(4)
                .ToListAsync();

            return View(post);
        }
    }
}
