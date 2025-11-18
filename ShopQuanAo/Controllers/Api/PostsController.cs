using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopQuanAo.Data;
using ShopQuanAo.Models;

namespace ShopQuanAo.Controllers.Api
{
    /// <summary>
    /// API bài viết (blog): danh sách và chi tiết.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class PostsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PostsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>Danh sách tất cả bài viết (mới nhất trước).</summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Post>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Post>>> GetAll()
        {
            var posts = await _context.Posts
                .AsNoTracking()
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return Ok(posts);
        }

        /// <summary>Lấy chi tiết bài viết theo ID.</summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(Post), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Post>> GetById(int id)
        {
            var post = await _context.Posts
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null) return NotFound();
            return Ok(post);
        }

        /// <summary>Lấy chi tiết bài viết theo slug.</summary>
        [HttpGet("slug/{slug}")]
        [ProducesResponseType(typeof(Post), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Post>> GetBySlug(string slug)
        {
            if (string.IsNullOrEmpty(slug))
                return BadRequest(new { message = "Slug không hợp lệ." });

            var post = await _context.Posts
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Slug == slug);

            if (post == null) return NotFound();
            return Ok(post);
        }
    }
}
