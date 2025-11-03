using ShopQuanAo.Models;           
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShopQuanAo.Services
{
    public interface ICouponService
    {
        Task<(bool ok, string message, decimal discount, Coupon? coupon)>
            TryApplyAsync(string inputCode, IEnumerable<CartItem> cartItems);
    }
}
