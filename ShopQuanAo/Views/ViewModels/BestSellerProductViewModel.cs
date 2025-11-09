using ShopQuanAo.Models;

namespace ShopQuanAo.ViewModels
{
    public class BestSellerProductViewModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; }
        public decimal? OldPrice { get; set; }
        public string? Category { get; set; }

        // Thống kê
        public int SoldQuantity { get; set; }          // Tổng số lượng đã bán
        public decimal Revenue { get; set; }           // Doanh thu (Price * Quantity hoặc LineTotal)

        // Giống Product để hiển thị
        public bool IsAvailable { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsOnSale { get; set; }

        // Helper cho ảnh mặc định
        public string DisplayImageUrl =>
            string.IsNullOrWhiteSpace(ImageUrl)
                ? "~/images/product_1.png"
                : ImageUrl;
    }
}
