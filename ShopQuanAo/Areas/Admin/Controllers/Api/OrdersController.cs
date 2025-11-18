using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopQuanAo.Data;
using ShopQuanAo.Models;

namespace ShopQuanAo.Areas.Admin.Controllers.Api
{
    /// <summary>
    /// API quản lý đơn hàng (Orders) dành cho Admin.
    /// </summary>
    [Area("Admin")]
    [ApiController]
    [Route("api/admin/[controller]")]
    [Authorize(Roles = "Admin")]
    [Produces("application/json")]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public OrdersController(ApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Danh sách đơn hàng (có filter & phân trang).
        /// </summary>
        /// <param name="status">Lọc theo trạng thái (Pending / Paid / Shipping / Delivered / Cancelled ...)</param>
        /// <param name="code">Lọc theo mã đơn (OrderCode chứa chuỗi này)</param>
        /// <param name="customer">Lọc theo tên khách (CustomerName chứa chuỗi này)</param>
        /// <param name="page">Trang hiện tại (>=1)</param>
        /// <param name="pageSize">Số đơn / trang (1–100)</param>
        [HttpGet]
        [ProducesResponseType(typeof(AdminOrderListResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<AdminOrderListResponse>> GetList(
            [FromQuery] string? status,
            [FromQuery] string? code,
            [FromQuery] string? customer,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var query = _db.Orders.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
            {
                var s = status.Trim();
                query = query.Where(o => o.Status == s);
            }

            if (!string.IsNullOrWhiteSpace(code))
            {
                var c = code.Trim();
                query = query.Where(o => o.OrderCode.Contains(c));
            }

            if (!string.IsNullOrWhiteSpace(customer))
            {
                var cu = customer.Trim();
                query = query.Where(o => o.CustomerName.Contains(cu));
            }

            // Tổng số bản ghi
            var totalItems = await query.CountAsync();

            // Giới hạn pageSize
            pageSize = pageSize switch
            {
                < 1 => 10,
                > 100 => 100,
                _ => pageSize
            };

            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            page = Math.Clamp(page, 1, Math.Max(1, totalPages));

            var orders = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new AdminOrderListItemDto
                {
                    Id = o.Id,
                    OrderCode = o.OrderCode,
                    CustomerName = o.CustomerName,
                    Phone = o.Phone,
                    Total = o.Total,
                    Status = o.Status,
                    PaymentMethod = o.PaymentMethod,
                    CreatedAt = o.CreatedAt,
                    UpdatedAt = o.UpdatedAt
                })
                .ToListAsync();

            var res = new AdminOrderListResponse
            {
                Items = orders,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                Status = status,
                Code = code,
                Customer = customer
            };

            return Ok(res);
        }

        /// <summary>
        /// Chi tiết 1 đơn hàng (kèm danh sách OrderItems).
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(AdminOrderDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AdminOrderDetailDto>> GetDetail(int id)
        {
            var order = await _db.Orders
                .Include(o => o.OrderItems)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound(new { message = "Không tìm thấy đơn hàng." });

            var dto = new AdminOrderDetailDto
            {
                Id = order.Id,
                OrderCode = order.OrderCode,
                CustomerName = order.CustomerName,
                Phone = order.Phone,
                Address = order.Address,
                Note = order.Note,
                PaymentMethod = order.PaymentMethod,
                Subtotal = order.Subtotal,
                Discount = order.Discount,
                ShippingFee = order.ShippingFee,
                Total = order.Total,
                Status = order.Status,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
                ApplicationUserId = order.ApplicationUserId,
                Items = order.OrderItems.Select(i => new AdminOrderItemDto
                {
                    Id = i.Id,
                    ProductName = i.ProductName,
                    ProductImage = i.ProductImage,
                    Price = i.Price,
                    Quantity = i.Quantity,
                    LineTotal = i.LineTotal
                }).ToList()
            };

            return Ok(dto);
        }

        /// <summary>
        /// Cập nhật trạng thái đơn hàng.
        /// </summary>
        [HttpPost("{id:int}/status")]
        [ProducesResponseType(typeof(AdminOrderDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AdminOrderDetailDto>> UpdateStatus(
            int id,
            [FromBody] UpdateOrderStatusDto model)
        {
            if (string.IsNullOrWhiteSpace(model.Status))
            {
                return BadRequest(new { message = "Trạng thái không được để trống." });
            }

            var o = await _db.Orders
                .Include(x => x.OrderItems)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (o == null)
                return NotFound(new { message = "Không tìm thấy đơn hàng." });

            // Bạn có thể thêm validate list status ở đây nếu muốn
            o.Status = model.Status.Trim();
            o.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            // map lại DTO detail
            var dto = new AdminOrderDetailDto
            {
                Id = o.Id,
                OrderCode = o.OrderCode,
                CustomerName = o.CustomerName,
                Phone = o.Phone,
                Address = o.Address,
                Note = o.Note,
                PaymentMethod = o.PaymentMethod,
                Subtotal = o.Subtotal,
                Discount = o.Discount,
                ShippingFee = o.ShippingFee,
                Total = o.Total,
                Status = o.Status,
                CreatedAt = o.CreatedAt,
                UpdatedAt = o.UpdatedAt,
                ApplicationUserId = o.ApplicationUserId,
                Items = o.OrderItems.Select(i => new AdminOrderItemDto
                {
                    Id = i.Id,
                    ProductName = i.ProductName,
                    ProductImage = i.ProductImage,
                    Price = i.Price,
                    Quantity = i.Quantity,
                    LineTotal = i.LineTotal
                }).ToList()
            };

            return Ok(dto);
        }

        /// <summary>
        /// Xoá đơn hàng (cả OrderItems).
        /// </summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _db.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound(new { message = "Không tìm thấy đơn hàng." });

            // nếu muốn xoá luôn items
            _db.OrderItems.RemoveRange(order.OrderItems);
            _db.Orders.Remove(order);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }

    // ===================== DTOs dùng cho Admin Order API =====================

    /// <summary>Item dùng cho list đơn hàng.</summary>
    public class AdminOrderListItemDto
    {
        public int Id { get; set; }
        public string OrderCode { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public string Status { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>Kết quả list có phân trang.</summary>
    public class AdminOrderListResponse
    {
        public List<AdminOrderListItemDto> Items { get; set; } = new();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }

        public string? Status { get; set; }
        public string? Code { get; set; }
        public string? Customer { get; set; }
    }

    /// <summary>Chi tiết đơn hàng + items.</summary>
    public class AdminOrderDetailDto
    {
        public int Id { get; set; }
        public string OrderCode { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? Note { get; set; }
        public string PaymentMethod { get; set; } = "COD";
        public decimal Subtotal { get; set; }
        public decimal Discount { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? ApplicationUserId { get; set; }

        public List<AdminOrderItemDto> Items { get; set; } = new();
    }

    /// <summary>Item trong đơn hàng.</summary>
    public class AdminOrderItemDto
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ProductImage { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal { get; set; }
    }

    /// <summary>Body cập nhật trạng thái đơn.</summary>
    public class UpdateOrderStatusDto
    {
        /// <summary>Trạng thái mới (Pending / Shipping / Delivered / Cancelled ...)</summary>
        public string Status { get; set; } = string.Empty;
    }
}
