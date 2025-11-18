using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopQuanAo.Data;
using ShopQuanAo.Models;

namespace ShopQuanAo.Areas.Admin.Controllers.Api
{
    [Area("Admin")]
    [ApiController]
    [Route("api/admin/slides")]
    [Authorize(Roles = "Admin")]
    [Produces("application/json")]
    public class SlideApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SlideApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>Danh sách tất cả slide.</summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Slide>>> GetAll()
        {
            var slides = await _context.Slides
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
            return Ok(slides);
        }

        /// <summary>Chi tiết 1 slide.</summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Slide>> GetById(int id)
        {
            var slide = await _context.Slides.FindAsync(id);
            if (slide == null) return NotFound();
            return Ok(slide);
        }

        /// <summary>Tạo slide mới (không upload file, chỉ data).</summary>
        [HttpPost]
        public async Task<ActionResult<Slide>> Create([FromBody] Slide model)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            model.Id = 0;
            model.CreatedAt = DateTime.Now;

            _context.Slides.Add(model);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = model.Id }, model);
        }

        /// <summary>Cập nhật slide.</summary>
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Slide>> Update(int id, [FromBody] Slide model)
        {
            if (id != model.Id) return BadRequest(new { message = "Id không khớp." });
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var slide = await _context.Slides.FindAsync(id);
            if (slide == null) return NotFound();

            slide.Title = model.Title;
            slide.Description = model.Description;
            slide.ImageUrl = model.ImageUrl;   // Với API mình cho phép sửa đường dẫn ảnh trực tiếp
            slide.IsActive = model.IsActive;

            await _context.SaveChangesAsync();
            return Ok(slide);
        }

        /// <summary>Xoá slide.</summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var slide = await _context.Slides.FindAsync(id);
            if (slide == null) return NotFound();

            _context.Slides.Remove(slide);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
