using Microsoft.AspNetCore.Http;
using ShopQuanAo.Models;

namespace ShopQuanAo.Services
{
    public interface IMomoService
    {
        Task<MomoCreatePaymentResponseModel> CreatePaymentAsync(OrderInfoModel model);
        MomoExecuteResponseModel PaymentExecuteAsync(IQueryCollection collection);
    }
}


