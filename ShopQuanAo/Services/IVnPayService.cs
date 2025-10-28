using Microsoft.AspNetCore.Http;
using ShopQuanAo.Models;

namespace ShopQuanAo.Services
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(PaymentInformationModel model, HttpContext ctx);
        PaymentResponseModel PaymentExecute(IQueryCollection query);
    }
}

