using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopQuanAo.Data;

namespace ShopQuanAo.Controllers.Api
{
    [ApiController]
    [Route("api/sale")]
    [Produces("application/json")]
    public class SaleApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SaleApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy danh sách sản phẩm đang giảm giá (IsOnSale = true).
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetSaleProducts()
        {
            var products = await _context.Products
                .AsNoTracking()
                .Where(p => p.IsOnSale && p.IsAvailable)
                .OrderByDescending(p => p.UpdatedDate ?? p.CreatedDate)
                .ToListAsync();

            return Ok(products);
        }
    }
}
