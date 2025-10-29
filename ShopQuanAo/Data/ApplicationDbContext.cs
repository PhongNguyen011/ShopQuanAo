using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShopQuanAo.Models;

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
        public DbSet<MomoTransaction> MomoTransactions { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed Roles
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

            // Seed Admin User
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
            // ===== Coupon config =====
            modelBuilder.Entity<Coupon>(e =>
            {
                e.HasIndex(x => x.Code).IsUnique(); // Code duy nhất
                e.Property(x => x.DiscountValue).HasColumnType("decimal(18,2)");
                e.Property(x => x.MinOrderAmount).HasColumnType("decimal(18,2)");
            });

            // ===== Order config =====
            modelBuilder.Entity<Order>(e =>
            {
                e.HasIndex(x => x.OrderCode).IsUnique(); // OrderCode duy nhất
                e.Property(x => x.Subtotal).HasColumnType("decimal(18,2)");
                e.Property(x => x.Discount).HasColumnType("decimal(18,2)");
                e.Property(x => x.Total).HasColumnType("decimal(18,2)");
                e.HasMany(x => x.OrderItems).WithOne(x => x.Order).HasForeignKey(x => x.OrderId).OnDelete(DeleteBehavior.Cascade);
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
            adminUser.PasswordHash = hasher.HashPassword(adminUser, "Admin@123");

            modelBuilder.Entity<ApplicationUser>().HasData(adminUser);

            // Assign Admin role to Admin user
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    RoleId = adminRoleId,
                    UserId = adminUserId
                }
            );

            // Optional: Slug unique Post
            modelBuilder.Entity<Post>()
                .HasIndex(p => p.Slug)
                .IsUnique(false);

            // Seed some sample data
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
        }
    }
}

