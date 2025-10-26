using Microsoft.AspNetCore.Mvc;
using ShopQuanAo.Data;
using ShopQuanAo.Models;

namespace ShopQuanAo.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SlideController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SlideController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ====================== INDEX ======================
        public IActionResult Index()
        {
            var slides = _context.Slides.ToList();
            return View(slides);
        }

        // ====================== CREATE ======================
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Slide slide, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "images", "slides");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        imageFile.CopyTo(fileStream);
                    }

                    slide.ImageUrl = "/images/slides/" + uniqueFileName;
                }

                slide.CreatedAt = DateTime.Now;
                _context.Slides.Add(slide);
                _context.SaveChanges();

                TempData["success"] = "Thêm slide thành công!";
                return RedirectToAction(nameof(Index));
            }

            TempData["error"] = "Vui lòng kiểm tra lại thông tin!";
            return View(slide);
        }

        // ====================== EDIT ======================
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var slide = _context.Slides.Find(id);
            if (slide == null)
                return NotFound();

            return View(slide);
        }
        [HttpPost]
        public IActionResult Edit(Slide slide, IFormFile? imageFile)
        {
            // Nếu không có file mới, bỏ validation cho ImageUrl
            if (imageFile == null || imageFile.Length == 0)
            {
                ModelState.Remove(nameof(slide.ImageUrl));
            }

            if (ModelState.IsValid)
            {
                var existingSlide = _context.Slides.Find(slide.Id);
                if (existingSlide == null)
                    return NotFound();

                // Cập nhật các trường text
                existingSlide.Title = slide.Title;
                existingSlide.Description = slide.Description;
                existingSlide.IsActive = slide.IsActive;

                // Nếu có file mới => upload và thay thế ảnh cũ
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Xóa ảnh cũ nếu có
                    if (!string.IsNullOrEmpty(existingSlide.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(_env.WebRootPath, existingSlide.ImageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                        if (System.IO.File.Exists(oldImagePath))
                            System.IO.File.Delete(oldImagePath);
                    }

                    // Tạo thư mục lưu ảnh nếu chưa có
                    string uploadFolder = Path.Combine(_env.WebRootPath, "images", "slides");
                    if (!Directory.Exists(uploadFolder))
                        Directory.CreateDirectory(uploadFolder);

                    // Tạo tên file duy nhất
                    string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    string filePath = Path.Combine(uploadFolder, uniqueFileName);

                    // Lưu file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        imageFile.CopyTo(stream);
                    }

                    // Cập nhật đường dẫn ảnh
                    existingSlide.ImageUrl = "/images/slides/" + uniqueFileName;
                }
                // Nếu không có file mới -> giữ nguyên ảnh cũ

                _context.Update(existingSlide);
                _context.SaveChanges();

                TempData["success"] = "Cập nhật slide thành công!";
                return RedirectToAction(nameof(Index));
            }

            // Nếu có lỗi, vẫn trả lại form với ảnh hiện tại
            var slideForView = _context.Slides.Find(slide.Id) ?? slide;
            return View(slideForView);
        }


        // ====================== DELETE ======================
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var slide = _context.Slides.Find(id);
            if (slide == null)
                return NotFound();

            // Xóa ảnh vật lý nếu có
            if (!string.IsNullOrEmpty(slide.ImageUrl))
            {
                var fullPath = Path.Combine(_env.WebRootPath, slide.ImageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                if (System.IO.File.Exists(fullPath))
                    System.IO.File.Delete(fullPath);
            }

            _context.Slides.Remove(slide);
            _context.SaveChanges();

            TempData["success"] = "Đã xóa slide thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}
