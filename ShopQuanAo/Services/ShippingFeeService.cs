using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShopQuanAo.Data;

namespace ShopQuanAo.Services
{
    public class ShippingFeeService : IShippingFeeService
    {
        private readonly ApplicationDbContext _context;

        public ShippingFeeService(ApplicationDbContext context)
        {
            _context = context;
        }

        private static string Normalize(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "";
            var n = s.Trim().ToLowerInvariant();

            // Bỏ prefix để khớp với tên từ API dễ hơn
            n = n.Replace("tỉnh ", "")
                 .Replace("thành phố ", "")
                 .Replace("tp. ", "")
                 .Replace("tp ", "");

            return n;
        }

        public async Task<decimal> GetFeeForProvinceAsync(string? provinceName)
        {
            if (string.IsNullOrWhiteSpace(provinceName))
                return 0m;

            var norm = Normalize(provinceName);

            // Lấy tất cả record active, rồi so sánh bằng Normalize
            var all = await _context.ShippingFees
                                    .Where(sf => sf.IsActive)
                                    .ToListAsync();

            var match = all.FirstOrDefault(sf =>
                Normalize(sf.ProvinceName) == norm);

            return match?.Fee ?? 0m;
        }
    }
}
