using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopQuanAo.Data;

namespace ShopQuanAo.ViewComponents
{
    public class FlashSaleHeroViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public FlashSaleHeroViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var now = DateTime.Now;

            // DỌN DEAL HẾT HẠN
            var expired = await _context.FlashSaleItems
                .Where(x => x.EndTime < now)
                .ToListAsync();
            if (expired.Any())
            {
                _context.FlashSaleItems.RemoveRange(expired);
                await _context.SaveChangesAsync();
            }

            // LOAD TẤT CẢ DEAL CÒN HẠN
            var deals = await _context.FlashSaleItems
                .Include(x => x.Product)
                .Where(x =>
                    x.IsActive &&
                    x.EndTime > now &&
                    x.Product.IsAvailable &&
                    x.Product.StockQuantity > 0
                )
                .OrderBy(x => x.EndTime) // sắp hết hạn lên trước
                .ToListAsync();

            if (!deals.Any())
            {
                return View("_FlashSaleUserEmpty");
            }

            // model bây giờ là list
            return View("Default", deals);
        }
    }
}
