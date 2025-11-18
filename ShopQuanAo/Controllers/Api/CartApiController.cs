using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopQuanAo.Data;
using ShopQuanAo.Models;
using ShopQuanAo.Models.Api;
using ShopQuanAo.Services;
using ShopQuanAo.Utils;

// Alias cho kiểu AppliedCouponInfo lồng trong CartController MVC
using CartAppliedCoupon = ShopQuanAo.Controllers.CartController.AppliedCouponInfo;

namespace ShopQuanAo.Controllers.Api
{
    /// <summary>
    /// API thao tác với giỏ hàng (cart).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CartController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly ICartService _cart;
        private readonly ICouponService _coupon;
        private const string COUPON_KEY = "CART_COUPON";

        public CartController(ApplicationDbContext db, ICartService cart, ICouponService coupon)
        {
            _db = db;
            _cart = cart;
            _coupon = coupon;
        }

        // ------- HELPER: coupon trong session -------

        private CartAppliedCoupon? GetAppliedCoupon()
            => HttpContext.Session.GetObject<CartAppliedCoupon>(COUPON_KEY);

        private void SetAppliedCoupon(CartAppliedCoupon? info)
        {
            if (info == null) HttpContext.Session.Remove(COUPON_KEY);
            else HttpContext.Session.SetObject(COUPON_KEY, info);
        }

        private async Task<(decimal subtotal, decimal discount, decimal total)> RecalcTotalsWithCouponAsync(List<CartItem> cart)
        {
            var subtotal = cart.Sum(x => x.LineTotal);
            var applied = GetAppliedCoupon();
            decimal discount = 0m;

            if (applied != null)
            {
                var re = await _coupon.TryApplyAsync(applied.Code, cart);
                if (re.ok && re.coupon != null)
                {
                    applied.Discount = re.discount;
                    applied.DiscountType = re.coupon.DiscountType;
                    applied.DiscountValue = re.coupon.DiscountValue;
                    applied.MinOrderAmount = re.coupon.MinOrderAmount;
                    applied.Scope = re.coupon.Scope;
                    applied.StartDate = re.coupon.StartDate;
                    applied.EndDate = re.coupon.EndDate;
                    applied.IsActive = re.coupon.IsActive;
                    applied.AllowedCategoriesCsv = re.coupon.AllowedCategoriesCsv;
                    SetAppliedCoupon(applied);
                    discount = re.discount;
                }
                else
                {
                    // không còn đủ điều kiện / mã hết hạn
                    SetAppliedCoupon(null);
                }
            }

            if (discount > subtotal) discount = subtotal;
            var total = subtotal - discount;
            return (subtotal, discount, total);
        }

        private CartResponseDto MapCart(List<CartItem> cart, decimal subtotal, decimal discount, decimal total)
        {
            var applied = GetAppliedCoupon();
            return new CartResponseDto
            {
                Items = cart.Select(x => new CartItemDto
                {
                    ProductId = x.ProductId,
                    Name = x.Name,
                    Price = x.Price,
                    Image = x.Image,
                    Quantity = x.Quantity
                }).ToList(),
                Summary = new CartSummaryDto
                {
                    Subtotal = subtotal,
                    Discount = discount,
                    Total = total
                },
                AppliedCouponCode = applied?.Code,
                AppliedCouponMessage = applied?.Message
            };
        }

        // ------- API -------

        /// <summary>Lấy toàn bộ giỏ hàng hiện tại.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(CartResponseDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<CartResponseDto>> Get()
        {
            var cart = _cart.GetCart();
            var (subtotal, discount, total) = await RecalcTotalsWithCouponAsync(cart);
            return Ok(MapCart(cart, subtotal, discount, total));
        }

        /// <summary>Thêm sản phẩm vào giỏ.</summary>
        [HttpPost("add")]
        [ProducesResponseType(typeof(CartResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CartResponseDto>> Add([FromBody] CartAddRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var p = await _db.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.ProductId);
            if (p == null) return NotFound(new { message = "Không tìm thấy sản phẩm." });

            var flashSale = await _db.FlashSaleItems
                .FirstOrDefaultAsync(f => f.ProductId == p.Id && f.IsActive && f.EndTime > DateTime.Now);

            var effectivePrice = flashSale != null ? flashSale.FlashPrice : p.Price;

            var cart = _cart.GetCart();
            var line = cart.FirstOrDefault(x => x.ProductId == p.Id);

            var stock = p.StockQuantity;
            if (stock <= 0)
                return BadRequest(new { message = $"Sản phẩm “{p.Name}” đã hết hàng." });

            if (line == null)
            {
                var addQty = Math.Min(request.Quantity, stock);
                cart.Add(new CartItem
                {
                    ProductId = p.Id,
                    Name = p.Name,
                    Price = effectivePrice,
                    Image = p.ImageUrl,
                    Quantity = addQty
                });
            }
            else
            {
                line.Quantity = Math.Min(line.Quantity + request.Quantity, stock);
                line.Price = effectivePrice;
            }

            _cart.SaveCart(cart);
            var (subtotal, discount, total) = await RecalcTotalsWithCouponAsync(cart);
            return Ok(MapCart(cart, subtotal, discount, total));
        }

        /// <summary>Thay đổi số lượng 1 dòng (delta +1/-1...).</summary>
        [HttpPost("change-qty")]
        [ProducesResponseType(typeof(CartChangeQtyResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<CartChangeQtyResponse>> ChangeQty([FromBody] CartChangeQtyRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new CartChangeQtyResponse { Ok = false, Error = "Dữ liệu không hợp lệ." });

            var cart = _cart.GetCart();
            var line = cart.FirstOrDefault(x => x.ProductId == request.ProductId);
            if (line == null)
            {
                return Ok(new CartChangeQtyResponse
                {
                    Ok = false,
                    Error = "Item not found"
                });
            }

            var stock = await _db.Products.Where(p => p.Id == request.ProductId)
                                          .Select(p => p.StockQuantity)
                                          .FirstOrDefaultAsync();

            if (stock <= 0)
            {
                cart.RemoveAll(x => x.ProductId == request.ProductId);
                _cart.SaveCart(cart);
                var (subtotal0, discount0, total0) = await RecalcTotalsWithCouponAsync(cart);
                return Ok(new CartChangeQtyResponse
                {
                    Ok = true,
                    Removed = true,
                    Subtotal = subtotal0,
                    Discount = discount0,
                    Total = total0,
                    Stock = 0
                });
            }

            var newQty = line.Quantity + request.Delta;
            if (newQty < 1) newQty = 1;
            if (newQty > stock) newQty = stock;
            line.Quantity = newQty;

            _cart.SaveCart(cart);
            var (subtotal, discount, total) = await RecalcTotalsWithCouponAsync(cart);

            return Ok(new CartChangeQtyResponse
            {
                Ok = true,
                Removed = false,
                Qty = line.Quantity,
                LineTotal = line.LineTotal,
                Subtotal = subtotal,
                Discount = discount,
                Total = total,
                Stock = stock,
                Maxed = line.Quantity >= stock,
                Mined = line.Quantity <= 1
            });
        }

        /// <summary>Áp dụng mã giảm giá (chọn mã giảm nhiều nhất nếu nhập nhiều).</summary>
        [HttpPost("apply-coupon")]
        [ProducesResponseType(typeof(ApplyCouponResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApplyCouponResponse>> ApplyCoupon([FromBody] ApplyCouponRequest req)
        {
            var cart = _cart.GetCart();
            if (cart.Count == 0)
            {
                SetAppliedCoupon(null);
                return Ok(new ApplyCouponResponse
                {
                    Ok = false,
                    Message = "Giỏ hàng trống."
                });
            }

            if (string.IsNullOrWhiteSpace(req.Code))
            {
                SetAppliedCoupon(null);
                return Ok(new ApplyCouponResponse
                {
                    Ok = false,
                    Message = "Vui lòng nhập mã."
                });
            }

            var codes = req.Code.Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(s => s.Trim().ToUpperInvariant())
                                .Distinct()
                                .ToList();

            CartAppliedCoupon? best = null;
            decimal bestDiscount = 0m;
            string bestMessage = "Không có mã hợp lệ.";

            foreach (var c in codes)
            {
                var re = await _coupon.TryApplyAsync(c, cart);
                if (re.ok && re.coupon != null && re.discount > bestDiscount)
                {
                    bestDiscount = re.discount;
                    bestMessage = re.message;
                    best = new CartAppliedCoupon
                    {
                        Code = re.coupon.Code,
                        Discount = re.discount,
                        Message = re.message,
                        DiscountType = re.coupon.DiscountType,
                        DiscountValue = re.coupon.DiscountValue,
                        MinOrderAmount = re.coupon.MinOrderAmount,
                        Scope = re.coupon.Scope,
                        StartDate = re.coupon.StartDate,
                        EndDate = re.coupon.EndDate,
                        IsActive = re.coupon.IsActive,
                        AllowedCategoriesCsv = re.coupon.AllowedCategoriesCsv
                    };
                }
            }

            var (subtotalAfter, discountAfter, totalAfter) = await RecalcTotalsWithCouponAsync(cart);

            if (best == null)
            {
                SetAppliedCoupon(null);
                return Ok(new ApplyCouponResponse
                {
                    Ok = false,
                    Message = bestMessage,
                    AppliedCode = null,
                    Subtotal = subtotalAfter,
                    Discount = discountAfter,
                    Total = totalAfter
                });
            }
            else
            {
                SetAppliedCoupon(best);
                // Recalc lại theo mã best
                var (subtotal, discount, total) = await RecalcTotalsWithCouponAsync(cart);
                return Ok(new ApplyCouponResponse
                {
                    Ok = true,
                    Message = $"Đã áp dụng mã {best.Code}. {best.Message}",
                    AppliedCode = best.Code,
                    Subtotal = subtotal,
                    Discount = discount,
                    Total = total
                });
            }
        }

        /// <summary>Gỡ mã giảm giá đang áp dụng.</summary>
        [HttpPost("remove-coupon")]
        [ProducesResponseType(typeof(RemoveCouponResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<RemoveCouponResponse>> RemoveCoupon()
        {
            SetAppliedCoupon(null);
            var cart = _cart.GetCart();
            var (subtotal, discount, total) = await RecalcTotalsWithCouponAsync(cart);
            return Ok(new RemoveCouponResponse
            {
                Ok = true,
                Subtotal = subtotal,
                Discount = discount,
                Total = total
            });
        }

        /// <summary>Xoá 1 sản phẩm khỏi giỏ.</summary>
        [HttpDelete("{productId:int}")]
        [ProducesResponseType(typeof(CartResponseDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<CartResponseDto>> Remove(int productId)
        {
            var cart = _cart.GetCart();
            cart.RemoveAll(x => x.ProductId == productId);
            _cart.SaveCart(cart);
            var (subtotal, discount, total) = await RecalcTotalsWithCouponAsync(cart);
            return Ok(MapCart(cart, subtotal, discount, total));
        }

        /// <summary>Xoá toàn bộ giỏ hàng.</summary>
        [HttpDelete("clear")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult Clear()
        {
            _cart.SaveCart(new List<CartItem>());
            SetAppliedCoupon(null);
            return NoContent();
        }

        /// <summary>Đếm số lượng item trong giỏ.</summary>
        [HttpGet("count")]
        [ProducesResponseType(typeof(CartCountResponse), StatusCodes.Status200OK)]
        public ActionResult<CartCountResponse> Count()
        {
            var n = _cart.Count();
            return Ok(new CartCountResponse { Count = n });
        }
    }
}
