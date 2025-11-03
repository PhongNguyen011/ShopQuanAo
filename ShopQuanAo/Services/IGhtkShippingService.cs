using System.Threading.Tasks;

namespace ShopQuanAo.Services
{
    public interface IGhtkShippingService
    {
        Task<int?> CalculateShippingFeeAsync(
            string toProvinceName,
            string toDistrictName,
            string toWardName,
            string toAddressDetail,
            int weightGram,
            int insuranceValueVnd);
    }
}


