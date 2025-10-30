using System.Threading.Tasks;

namespace ShopQuanAo.Services
{
    public interface IGhnShippingService
    {
        Task<int?> CalculateShippingFeeAsync(
            int toDistrictId,
            string toWardCode,
            int weightGram,
            int lengthCm,
            int widthCm,
            int heightCm,
            int insuranceValue);
    }
}
