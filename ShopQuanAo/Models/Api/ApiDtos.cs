using System.ComponentModel.DataAnnotations;

namespace ShopQuanAo.Models.Api
{
    // ---------- CART ----------

    /// <summary>Item trong giỏ hàng.</summary>
    public class CartItemDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
        public string? Image { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal => Price * Quantity;
    }

    /// <summary>Tổng quan giỏ hàng.</summary>
    public class CartSummaryDto
    {
        public decimal Subtotal { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
    }

    /// <summary>Giỏ hàng + tổng tiền + thông tin mã giảm giá.</summary>
    public class CartResponseDto
    {
        public List<CartItemDto> Items { get; set; } = new();
        public CartSummaryDto Summary { get; set; } = new();
        public string? AppliedCouponCode { get; set; }
        public string? AppliedCouponMessage { get; set; }
    }

    public class CartAddRequest
    {
        [Required]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; } = 1;
    }

    public class CartChangeQtyRequest
    {
        [Required]
        public int ProductId { get; set; }

        /// <summary>Delta tăng/giảm (ví dụ +1, -1).</summary>
        [Range(-100, 100)]
        public int Delta { get; set; }
    }

    public class CartChangeQtyResponse
    {
        public bool Ok { get; set; }
        public bool Removed { get; set; }
        public int Qty { get; set; }
        public decimal LineTotal { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
        public int Stock { get; set; }
        public bool Maxed { get; set; }
        public bool Mined { get; set; }
        public string? Error { get; set; }
    }

    public class ApplyCouponRequest
    {
        [Required]
        public string Code { get; set; } = "";
    }

    public class ApplyCouponResponse
    {
        public bool Ok { get; set; }
        public string Message { get; set; } = "";
        public string? AppliedCode { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
    }

    public class RemoveCouponResponse
    {
        public bool Ok { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
    }

    public class CartCountResponse
    {
        public int Count { get; set; }
    }

    // ---------- SHIPPING ----------

    /// <summary>Địa chỉ giao hàng để tính phí ship.</summary>
    public class ShippingAddressDto
    {
        public int ProvinceId { get; set; }
        public string ProvinceName { get; set; } = "";
        public int DistrictId { get; set; }
        public string DistrictName { get; set; } = "";
        public int WardId { get; set; }
        public string WardName { get; set; } = "";
        public string AddressDetail { get; set; } = "";
    }

    public class ShippingFeeResponse
    {
        public bool Ok { get; set; }
        public int ShipFee { get; set; }
        public string Message { get; set; } = "";
    }

    // ---------- WISHLIST ----------

    public class WishlistItemDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class ToggleWishlistRequest
    {
        [Required]
        public int ProductId { get; set; }
    }

    public class ToggleWishlistResponse
    {
        public bool Ok { get; set; }
        public bool Favorited { get; set; }
    }

    // ---------- PRODUCT ----------

    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; }
        public decimal? OldPrice { get; set; }
        public string? Category { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsOnSale { get; set; }
        public bool IsFeatured { get; set; }
    }

    public class ProductListRequest
    {
        public string? Category { get; set; }
        public string? Q { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class PagedProductResponse
    {
        public List<ProductDto> Items { get; set; } = new();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }

    public class BestSellerProductDto : ProductDto
    {
        public int SoldQuantity { get; set; }
        public decimal Revenue { get; set; }
    }
}
