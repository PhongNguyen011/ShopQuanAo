using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShopQuanAo.Models;
using System;

namespace ShopQuanAo.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Slide> Slides { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<FlashSaleItem> FlashSaleItems { get; set; }
        public DbSet<MomoTransaction> MomoTransactions { get; set; }
        public DbSet<WishlistItem> WishlistItems { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }
        public DbSet<ShippingFee> ShippingFees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===== Seed Roles =====
            var adminRoleId = "1";
            var userRoleId = "2";

            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = adminRoleId,
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole
                {
                    Id = userRoleId,
                    Name = "User",
                    NormalizedName = "USER"
                }
            );

            // ===== Seed Admin User =====
            var hasher = new PasswordHasher<ApplicationUser>();
            var adminUserId = "admin-001";

            var adminUser = new ApplicationUser
            {
                Id = adminUserId,
                UserName = "admin@shopquanao.com",
                NormalizedUserName = "ADMIN@SHOPQUANAO.COM",
                Email = "admin@shopquanao.com",
                NormalizedEmail = "ADMIN@SHOPQUANAO.COM",
                EmailConfirmed = true,
                FullName = "Administrator",
                PhoneNumber = "0123456789",
                SecurityStamp = Guid.NewGuid().ToString(),
                CreatedDate = DateTime.Now,
                IsActive = true
            };

            adminUser.PasswordHash = hasher.HashPassword(adminUser, "Admin@123");

            modelBuilder.Entity<ApplicationUser>().HasData(adminUser);

            // Gán role Admin cho user admin
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    RoleId = adminRoleId,
                    UserId = adminUserId
                }
            );

            // ===== Coupon config =====
            modelBuilder.Entity<Coupon>(e =>
            {
                // Code duy nhất
                e.HasIndex(x => x.Code).IsUnique();
                e.Property(x => x.DiscountValue).HasColumnType("decimal(18,2)");
                e.Property(x => x.MinOrderAmount).HasColumnType("decimal(18,2)");
            });

            // ===== Order config =====
            modelBuilder.Entity<Order>(e =>
            {
                e.HasIndex(x => x.OrderCode).IsUnique();
                e.Property(x => x.Subtotal).HasColumnType("decimal(18,2)");
                e.Property(x => x.Discount).HasColumnType("decimal(18,2)");
                e.Property(x => x.Total).HasColumnType("decimal(18,2)");
                e.HasMany(x => x.OrderItems)
                 .WithOne(x => x.Order)
                 .HasForeignKey(x => x.OrderId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ===== OrderItem config =====
            modelBuilder.Entity<OrderItem>(e =>
            {
                e.Property(x => x.Price).HasColumnType("decimal(18,2)");
                e.Property(x => x.LineTotal).HasColumnType("decimal(18,2)");
            });

            // ===== MoMo Transaction config =====
            modelBuilder.Entity<MomoTransaction>(e =>
            {
                e.HasIndex(x => x.OrderId).IsUnique(false);
                e.Property(x => x.Amount).HasColumnType("decimal(18,2)");
                e.Property(x => x.Status).HasMaxLength(32);
            });

            // ===== Wishlist config =====
            modelBuilder.Entity<WishlistItem>(e =>
            {
                e.HasIndex(x => new { x.ApplicationUserId, x.ProductId }).IsUnique();
                e.HasOne(x => x.Product)
                 .WithMany()
                 .HasForeignKey(x => x.ProductId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ===== ShippingFee config =====
            modelBuilder.Entity<ShippingFee>(e =>
            {
                e.HasIndex(x => x.ProvinceName).IsUnique(false);
                e.Property(x => x.Fee).HasColumnType("decimal(18,2)");
            });

            // ===== Post config (slug) =====
            modelBuilder.Entity<Post>()
                .HasIndex(p => p.Slug)
                .IsUnique(false);

            // ===== Seed sample products =====
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Áo Thun Nam Basic",
                    Description = "Áo thun nam chất liệu cotton cao cấp, thoáng mát",
                    Price = 199000,
                    OldPrice = 250000,
                    ImageUrl = "~/images/product_1.png",
                    Category = "men",
                    IsAvailable = true,
                    IsFeatured = true,
                    IsOnSale = true,
                    StockQuantity = 100,
                    CreatedDate = DateTime.Now
                },
                new Product
                {
                    Id = 2,
                    Name = "Áo Sơ Mi Nữ",
                    Description = "Áo sơ mi nữ công sở sang trọng",
                    Price = 350000,
                    ImageUrl = "~/images/product_2.png",
                    Category = "women",
                    IsAvailable = true,
                    IsFeatured = true,
                    IsOnSale = false,
                    StockQuantity = 50,
                    CreatedDate = DateTime.Now
                },
                new Product
                {
                    Id = 3,
                    Name = "Quần Jeans Nam",
                    Description = "Quần jeans nam form slim fit",
                    Price = 450000,
                    ImageUrl = "~/images/product_3.png",
                    Category = "men",
                    IsAvailable = true,
                    IsFeatured = false,
                    IsOnSale = false,
                    StockQuantity = 75,
                    CreatedDate = DateTime.Now
                },
                new Product
                {
                    Id = 4,
                    Name = "Váy Đầm Nữ",
                    Description = "Váy đầm nữ thời trang cao cấp",
                    Price = 550000,
                    OldPrice = 650000,
                    ImageUrl = "~/images/product_4.png",
                    Category = "women",
                    IsAvailable = true,
                    IsFeatured = true,
                    IsOnSale = true,
                    StockQuantity = 30,
                    CreatedDate = DateTime.Now
                },
                new Product
                {
                    Id = 5,
                    Name = "Áo Khoác Nam",
                    Description = "Áo khoác nam phong cách Hàn Quốc",
                    Price = 650000,
                    ImageUrl = "~/images/product_5.png",
                    Category = "men",
                    IsAvailable = true,
                    IsFeatured = false,
                    IsOnSale = false,
                    StockQuantity = 45,
                    CreatedDate = DateTime.Now
                }
            );

            // ===== Seed 63 tỉnh/thành cho ShippingFees =====
            var seedDate = new DateTime(2025, 1, 1);

            modelBuilder.Entity<ShippingFee>().HasData(
                // --- Thành phố lớn – 35.000đ ---
                new ShippingFee { Id = 101, ProvinceName = "Thành phố Hà Nội", Fee = 35000m, IsActive = true, Description = "Nội thành & lân cận", CreatedAt = seedDate },
                new ShippingFee { Id = 102, ProvinceName = "Thành phố Hồ Chí Minh", Fee = 35000m, IsActive = true, Description = "Nội thành & lân cận", CreatedAt = seedDate },
                new ShippingFee { Id = 103, ProvinceName = "Thành phố Đà Nẵng", Fee = 35000m, IsActive = true, Description = "Nội thành & lân cận", CreatedAt = seedDate },
                new ShippingFee { Id = 104, ProvinceName = "Thành phố Hải Phòng", Fee = 35000m, IsActive = true, Description = "Nội thành & lân cận", CreatedAt = seedDate },
                new ShippingFee { Id = 105, ProvinceName = "Thành phố Cần Thơ", Fee = 35000m, IsActive = true, Description = "Nội thành & lân cận", CreatedAt = seedDate },

                // --- Miền núi / Tây Nguyên – 55.000đ ---
                new ShippingFee { Id = 106, ProvinceName = "Tỉnh Hà Giang", Fee = 55000m, IsActive = true, Description = "Tỉnh miền núi", CreatedAt = seedDate },
                new ShippingFee { Id = 107, ProvinceName = "Tỉnh Cao Bằng", Fee = 55000m, IsActive = true, Description = "Tỉnh miền núi", CreatedAt = seedDate },
                new ShippingFee { Id = 108, ProvinceName = "Tỉnh Bắc Kạn", Fee = 55000m, IsActive = true, Description = "Tỉnh miền núi", CreatedAt = seedDate },
                new ShippingFee { Id = 109, ProvinceName = "Tỉnh Tuyên Quang", Fee = 55000m, IsActive = true, Description = "Tỉnh miền núi", CreatedAt = seedDate },
                new ShippingFee { Id = 110, ProvinceName = "Tỉnh Lào Cai", Fee = 55000m, IsActive = true, Description = "Tỉnh miền núi", CreatedAt = seedDate },
                new ShippingFee { Id = 111, ProvinceName = "Tỉnh Điện Biên", Fee = 55000m, IsActive = true, Description = "Tỉnh miền núi", CreatedAt = seedDate },
                new ShippingFee { Id = 112, ProvinceName = "Tỉnh Lai Châu", Fee = 55000m, IsActive = true, Description = "Tỉnh miền núi", CreatedAt = seedDate },
                new ShippingFee { Id = 113, ProvinceName = "Tỉnh Sơn La", Fee = 55000m, IsActive = true, Description = "Tỉnh miền núi", CreatedAt = seedDate },
                new ShippingFee { Id = 114, ProvinceName = "Tỉnh Yên Bái", Fee = 55000m, IsActive = true, Description = "Tỉnh miền núi", CreatedAt = seedDate },
                new ShippingFee { Id = 115, ProvinceName = "Tỉnh Hòa Bình", Fee = 55000m, IsActive = true, Description = "Tỉnh miền núi", CreatedAt = seedDate },
                new ShippingFee { Id = 116, ProvinceName = "Tỉnh Lạng Sơn", Fee = 55000m, IsActive = true, Description = "Vùng núi phía Bắc", CreatedAt = seedDate },
                new ShippingFee { Id = 117, ProvinceName = "Tỉnh Kon Tum", Fee = 55000m, IsActive = true, Description = "Tây Nguyên", CreatedAt = seedDate },
                new ShippingFee { Id = 118, ProvinceName = "Tỉnh Gia Lai", Fee = 55000m, IsActive = true, Description = "Tây Nguyên", CreatedAt = seedDate },
                new ShippingFee { Id = 119, ProvinceName = "Tỉnh Đắk Lắk", Fee = 55000m, IsActive = true, Description = "Tây Nguyên", CreatedAt = seedDate },
                new ShippingFee { Id = 120, ProvinceName = "Tỉnh Đắk Nông", Fee = 55000m, IsActive = true, Description = "Tây Nguyên", CreatedAt = seedDate },
                new ShippingFee { Id = 121, ProvinceName = "Tỉnh Lâm Đồng", Fee = 55000m, IsActive = true, Description = "Tây Nguyên", CreatedAt = seedDate },

                // --- Các tỉnh còn lại – 45.000đ ---
                new ShippingFee { Id = 122, ProvinceName = "Tỉnh Quảng Ninh", Fee = 45000m, IsActive = true, Description = "Ven biển", CreatedAt = seedDate },
                new ShippingFee { Id = 123, ProvinceName = "Tỉnh Bắc Giang", Fee = 45000m, IsActive = true, Description = "", CreatedAt = seedDate },
                new ShippingFee { Id = 124, ProvinceName = "Tỉnh Phú Thọ", Fee = 45000m, IsActive = true, Description = "", CreatedAt = seedDate },
                new ShippingFee { Id = 125, ProvinceName = "Tỉnh Vĩnh Phúc", Fee = 45000m, IsActive = true, Description = "", CreatedAt = seedDate },
                new ShippingFee { Id = 126, ProvinceName = "Tỉnh Thái Nguyên", Fee = 45000m, IsActive = true, Description = "", CreatedAt = seedDate },
                new ShippingFee { Id = 127, ProvinceName = "Tỉnh Bắc Ninh", Fee = 45000m, IsActive = true, Description = "", CreatedAt = seedDate },
                new ShippingFee { Id = 128, ProvinceName = "Tỉnh Hải Dương", Fee = 45000m, IsActive = true, Description = "", CreatedAt = seedDate },
                new ShippingFee { Id = 129, ProvinceName = "Tỉnh Hưng Yên", Fee = 45000m, IsActive = true, Description = "", CreatedAt = seedDate },
                new ShippingFee { Id = 130, ProvinceName = "Tỉnh Thái Bình", Fee = 45000m, IsActive = true, Description = "", CreatedAt = seedDate },
                new ShippingFee { Id = 131, ProvinceName = "Tỉnh Hà Nam", Fee = 45000m, IsActive = true, Description = "", CreatedAt = seedDate },
                new ShippingFee { Id = 132, ProvinceName = "Tỉnh Nam Định", Fee = 45000m, IsActive = true, Description = "", CreatedAt = seedDate },
                new ShippingFee { Id = 133, ProvinceName = "Tỉnh Ninh Bình", Fee = 45000m, IsActive = true, Description = "", CreatedAt = seedDate },
                new ShippingFee { Id = 134, ProvinceName = "Tỉnh Thanh Hóa", Fee = 45000m, IsActive = true, Description = "", CreatedAt = seedDate },
                new ShippingFee { Id = 135, ProvinceName = "Tỉnh Nghệ An", Fee = 45000m, IsActive = true, Description = "", CreatedAt = seedDate },
                new ShippingFee { Id = 136, ProvinceName = "Tỉnh Hà Tĩnh", Fee = 45000m, IsActive = true, Description = "", CreatedAt = seedDate },
                new ShippingFee { Id = 137, ProvinceName = "Tỉnh Quảng Bình", Fee = 45000m, IsActive = true, Description = "", CreatedAt = seedDate },
                new ShippingFee { Id = 138, ProvinceName = "Tỉnh Quảng Trị", Fee = 45000m, IsActive = true, Description = "", CreatedAt = seedDate },
                new ShippingFee { Id = 139, ProvinceName = "Tỉnh Thừa Thiên Huế", Fee = 45000m, IsActive = true, Description = "", CreatedAt = seedDate },
                new ShippingFee { Id = 140, ProvinceName = "Tỉnh Quảng Nam", Fee = 45000m, IsActive = true, Description = "", CreatedAt = seedDate },
                new ShippingFee { Id = 141, ProvinceName = "Tỉnh Quảng Ngãi", Fee = 45000m, IsActive = true, Description = "", CreatedAt = seedDate },
                new ShippingFee { Id = 142, ProvinceName = "Tỉnh Bình Định", Fee = 45000m, IsActive = true, Description = "", CreatedAt = seedDate },
                new ShippingFee { Id = 143, ProvinceName = "Tỉnh Phú Yên", Fee = 45000m, IsActive = true, Description = "", CreatedAt = seedDate },
                new ShippingFee { Id = 144, ProvinceName = "Tỉnh Khánh Hòa", Fee = 45000m, IsActive = true, Description = "Ven biển", CreatedAt = seedDate },
                new ShippingFee { Id = 145, ProvinceName = "Tỉnh Ninh Thuận", Fee = 45000m, IsActive = true, Description = "Ven biển", CreatedAt = seedDate },
                new ShippingFee { Id = 146, ProvinceName = "Tỉnh Bình Thuận", Fee = 45000m, IsActive = true, Description = "Ven biển", CreatedAt = seedDate },
                new ShippingFee { Id = 147, ProvinceName = "Tỉnh Bình Phước", Fee = 45000m, IsActive = true, Description = "", CreatedAt = seedDate },
                new ShippingFee { Id = 148, ProvinceName = "Tỉnh Tây Ninh", Fee = 45000m, IsActive = true, Description = "", CreatedAt = seedDate },
                new ShippingFee { Id = 149, ProvinceName = "Tỉnh Bình Dương", Fee = 45000m, IsActive = true, Description = "", CreatedAt = seedDate },
                new ShippingFee { Id = 150, ProvinceName = "Tỉnh Đồng Nai", Fee = 45000m, IsActive = true, Description = "", CreatedAt = seedDate },
                new ShippingFee { Id = 151, ProvinceName = "Tỉnh Bà Rịa - Vũng Tàu", Fee = 45000m, IsActive = true, Description = "Ven biển", CreatedAt = seedDate },
                new ShippingFee { Id = 152, ProvinceName = "Tỉnh Long An", Fee = 45000m, IsActive = true, Description = "", CreatedAt = seedDate },
                new ShippingFee { Id = 153, ProvinceName = "Tỉnh Tiền Giang", Fee = 45000m, IsActive = true, Description = "", CreatedAt = seedDate },
                new ShippingFee { Id = 154, ProvinceName = "Tỉnh Bến Tre", Fee = 45000m, IsActive = true, Description = "", CreatedAt = seedDate },
                new ShippingFee { Id = 155, ProvinceName = "Tỉnh Trà Vinh", Fee = 45000m, IsActive = true, Description = "", CreatedAt = seedDate },
                new ShippingFee { Id = 156, ProvinceName = "Tỉnh Vĩnh Long", Fee = 45000m, IsActive = true, Description = "", CreatedAt = seedDate },
                new ShippingFee { Id = 157, ProvinceName = "Tỉnh Đồng Tháp", Fee = 45000m, IsActive = true, Description = "", CreatedAt = seedDate },
                new ShippingFee { Id = 158, ProvinceName = "Tỉnh An Giang", Fee = 45000m, IsActive = true, Description = "", CreatedAt = seedDate },
                new ShippingFee { Id = 159, ProvinceName = "Tỉnh Kiên Giang", Fee = 45000m, IsActive = true, Description = "Ven biển", CreatedAt = seedDate },
                new ShippingFee { Id = 160, ProvinceName = "Tỉnh Hậu Giang", Fee = 45000m, IsActive = true, Description = "", CreatedAt = seedDate },
                new ShippingFee { Id = 161, ProvinceName = "Tỉnh Sóc Trăng", Fee = 45000m, IsActive = true, Description = "Ven biển", CreatedAt = seedDate },
                new ShippingFee { Id = 162, ProvinceName = "Tỉnh Bạc Liêu", Fee = 45000m, IsActive = true, Description = "Ven biển", CreatedAt = seedDate },
                new ShippingFee { Id = 163, ProvinceName = "Tỉnh Cà Mau", Fee = 45000m, IsActive = true, Description = "Ven biển", CreatedAt = seedDate }
            );
        }
    }
}
