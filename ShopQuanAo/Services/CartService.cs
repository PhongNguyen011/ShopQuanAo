using ShopQuanAo.Data.Common;
using ShopQuanAo.Models;

namespace ShopQuanAo.Services
{
    public interface ICartService
    {
        List<CartItem> GetCart();
        void SaveCart(List<CartItem> items);
        int Count();
    }

    public class CartService : ICartService
    {
        private readonly IHttpContextAccessor _http;
        private const string CART_KEY = "CART";

        public CartService(IHttpContextAccessor http) => _http = http;

        public List<CartItem> GetCart()
        {
            var session = _http.HttpContext!.Session;
            return session.GetObject<List<CartItem>>(CART_KEY) ?? new List<CartItem>();
        }

        public void SaveCart(List<CartItem> items)
        {
            var session = _http.HttpContext!.Session;
            session.SetObject(CART_KEY, items);
        }

        public int Count() => GetCart().Sum(x => x.Quantity);
    }
}
