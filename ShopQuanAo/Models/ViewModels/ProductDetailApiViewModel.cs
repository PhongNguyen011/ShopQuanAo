using ShopQuanAo.Models;

namespace ShopQuanAo.ViewModels
{
    public class ProductDetailApiViewModel
    {
        public Product Product { get; set; } = default!;
        public decimal DisplayPrice { get; set; }
        public bool IsFlashSale { get; set; }
        public FlashSaleItem? FlashSaleItem { get; set; }
        public IEnumerable<Product> RelatedProducts { get; set; } = Enumerable.Empty<Product>();
    }
}
