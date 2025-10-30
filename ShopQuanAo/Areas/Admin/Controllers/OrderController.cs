using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopQuanAo.Data;
using ShopQuanAo.Models;

namespace ShopQuanAo.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _db;
        public OrderController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var orders = _db.Orders.OrderByDescending(x => x.CreatedAt).ToList();
            return View(orders);
        }

        [HttpPost]
        public IActionResult UpdateStatus(int id, string status)
        {
            var o = _db.Orders.FirstOrDefault(x => x.Id == id);
            if (o == null) return NotFound();
            o.Status = status;
            o.UpdatedAt = DateTime.UtcNow;
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
