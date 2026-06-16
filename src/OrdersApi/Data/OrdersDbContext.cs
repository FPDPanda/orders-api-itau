using Microsoft.EntityFrameworkCore;
using OrdersApi.Domain.Models;

namespace OrdersApi.Data;

public class OrdersDbContext : DbContext
{
    public OrdersDbContext(DbContextOptions<OrdersDbContext> options) : base(options)
    {
    }

    public DbSet<Order> Orders { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Discount> Discounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(o => o.Id);
            entity.Property(o => o.Type).HasConversion<string>();
            entity.Property(o => o.Status).HasConversion<string>();
            entity.Property(o => o.OriginalValue).HasColumnType("decimal(18,2)");
            entity.Property(o => o.DebitedValue).HasColumnType("decimal(18,2)");
            entity.HasMany(o => o.Products)
                  .WithOne()
                  .HasForeignKey(p => p.OrderId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(d => d.Id);
            entity.Property(d => d.OrderType).HasConversion<string>();
            entity.Property(d => d.DiscountType).HasConversion<string>();
            entity.Property(d => d.Rate).HasColumnType("decimal(10,4)");
        });
    }
}
