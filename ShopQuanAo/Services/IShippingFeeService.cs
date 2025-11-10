using System.Threading.Tasks;

namespace ShopQuanAo.Services
{
    public interface IShippingFeeService
    {
        /// <summary>
        /// Lấy phí ship theo tên tỉnh/thành (từ API địa chỉ / form).
        /// Nếu không tìm thấy thì trả về 0.
        /// </summary>
        Task<decimal> GetFeeForProvinceAsync(string? provinceName);
    }
}
