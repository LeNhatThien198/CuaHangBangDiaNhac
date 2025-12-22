using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CuaHangBangDiaNhac.Models;

namespace CuaHangBangDiaNhac.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Brand> Brands { get; set; } = null!;

        public DbSet<Genre> Genres { get; set; } = null!;
        public DbSet<Style> Styles { get; set; } = null!;
        public DbSet<Artist> Artists { get; set; } = null!;

        public DbSet<ProductImage> ProductImages { get; set; } = null!;

        public DbSet<Cart> Carts { get; set; } = null!;
        public DbSet<CartItem> CartItems { get; set; } = null!;

        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;

        public DbSet<Address> Addresses { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName != null && tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }

            builder.Entity<Product>().Property(p => p.Price).HasPrecision(18, 2);
            builder.Entity<Product>().Property(p => p.Cost).HasPrecision(18, 2); 
            builder.Entity<Order>().Property(o => o.ShippingFee).HasPrecision(18, 2);
            builder.Entity<Order>().Property(o => o.Discount).HasPrecision(18, 2);
            builder.Entity<Order>().Property(o => o.DepositAmount).HasPrecision(18, 2);
            builder.Entity<OrderItem>().Property(oi => oi.UnitPrice).HasPrecision(18, 2);

            builder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull); 

            builder.Entity<Address>()
                .HasOne(a => a.User)
                .WithMany(u => u.Addresses)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Style>()
                .HasOne(s => s.Genre)
                .WithMany(g => g.Styles)
                .HasForeignKey(s => s.GenreId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Product>()
                .HasOne(p => p.Style)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.StyleId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.Items)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade); 

            builder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany()
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<IdentityRole>().HasData(
                new IdentityRole()
                {
                    Id = "1",
                    Name = "Admin",
                    NormalizedName = "ADMIN", 
                },
                new IdentityRole()
                {
                    Id = "2",
                    Name = "Staff",
                    NormalizedName = "STAFF",
                },
                new IdentityRole()
                {
                    Id = "3",
                    Name = "Customer",
                    NormalizedName = "CUSTOMER", 
                }
            );
        }
    }
}