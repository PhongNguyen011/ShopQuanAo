using ShopQuanAo.Models;           // giữ 1 dòng thôi
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShopQuanAo.Services
{
    // ✅ phải là interface, KHÔNG phải class
    public interface ICouponService
    {
        // ✅ trong interface chỉ khai báo chữ ký + dấu ;
        Task<(bool ok, string message, decimal discount, Coupon? coupon)>
            TryApplyAsync(string inputCode, IEnumerable<CartItem> cartItems);
    }
}
