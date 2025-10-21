using Microsoft.EntityFrameworkCore;
using ShopQuanAo.Models;

namespace ShopQuanAo.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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

