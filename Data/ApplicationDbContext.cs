using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CuaHangBangDiaNhac.Models;

namespace CuaHangBangDiaNhac.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

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
        public DbSet<Track> Tracks { get; set; } = null!;
        public DbSet<ReleaseVersion> ReleaseVersions { get; set; } = null!;
        public DbSet<DigitalAsset> DigitalAssets { get; set; } = null!;
        public DbSet<AuditLog> AuditLogs { get; set; } = null!;
        public DbSet<ModeratorTicket> ModeratorTickets { get; set; } = null!;
        public DbSet<UserSupportTicket> UserSupportTickets { get; set; } = null!;

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries();
            
            foreach (var entry in entries)
            {
                // Note: Auto-audit logging framework
                // Full implementation deferred to Phase 3 to avoid circular dependency
                // with AuditLogService. Currently using manual logging in Controllers/Services.
                
                if (entry.State == EntityState.Added)
                {
                    if (entry.Entity is IAuditableEntity auditableEntity)
                    {
                        auditableEntity.CreatedAt = DateTime.UtcNow;
                    }
                }
                else if (entry.State == EntityState.Modified)
                {
                    if (entry.Entity is IAuditableEntity auditableEntity)
                    {
                        auditableEntity.UpdatedAt = DateTime.UtcNow;
                    }
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

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

            // Configure decimal precision for monetary fields
            builder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            builder.Entity<Product>()
                .Property(p => p.PromotionPrice)
                .HasPrecision(18, 2);

            builder.Entity<Product>()
                .Property(p => p.Cost)
                .HasPrecision(18, 2);

            builder.Entity<Order>().Property(o => o.ShippingFee).HasPrecision(18, 2);
            builder.Entity<Order>().Property(o => o.Discount).HasPrecision(18, 2);
            builder.Entity<Order>().Property(o => o.DepositAmount).HasPrecision(18, 2);
            builder.Entity<OrderItem>().Property(oi => oi.UnitPrice).HasPrecision(18, 2);

            // ReleaseVersion → Product (Restrict on delete)
            builder.Entity<ReleaseVersion>()
                .HasOne(rv => rv.Product)
                .WithMany(p => p.ReleaseVersions)
                .HasForeignKey(rv => rv.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // ReleaseVersion → Track (Cascade delete)
            builder.Entity<ReleaseVersion>()
                .HasMany(rv => rv.Tracks)
                .WithOne(t => t.ReleaseVersion)
                .HasForeignKey(t => t.ReleaseVersionId)
                .OnDelete(DeleteBehavior.Cascade);

            // ReleaseVersion → DigitalAsset (Cascade delete)
            builder.Entity<ReleaseVersion>()
                .HasOne(rv => rv.DigitalAsset)
                .WithOne(da => da.ReleaseVersion)
                .HasForeignKey<DigitalAsset>(da => da.ReleaseVersionId)
                .OnDelete(DeleteBehavior.Cascade);

            // ModeratorTicket → Product (Restrict on delete)
            builder.Entity<ModeratorTicket>()
                .HasOne(mt => mt.Product)
                .WithMany()
                .HasForeignKey(mt => mt.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // ModeratorTicket → User (Moderator)
            builder.Entity<ModeratorTicket>()
                .HasOne(mt => mt.Moderator)
                .WithMany()
                .HasForeignKey(mt => mt.ModeratorId)
                .OnDelete(DeleteBehavior.SetNull);

            // UserSupportTicket → User (Creator)
            builder.Entity<UserSupportTicket>()
                .HasOne(ust => ust.User)
                .WithMany()
                .HasForeignKey(ust => ust.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // UserSupportTicket → User (AssignedTo)
            builder.Entity<UserSupportTicket>()
                .HasOne(ust => ust.AssignedTo)
                .WithMany()
                .HasForeignKey(ust => ust.AssignedToId)
                .OnDelete(DeleteBehavior.SetNull);

            // AuditLog → User
            builder.Entity<AuditLog>()
                .HasOne(al => al.User)
                .WithMany()
                .HasForeignKey(al => al.UserId)
                .OnDelete(DeleteBehavior.SetNull);

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

    // Marker interface for auditable entities
    public interface IAuditableEntity
    {
        DateTime CreatedAt { get; set; }
        DateTime? UpdatedAt { get; set; }
    }
}