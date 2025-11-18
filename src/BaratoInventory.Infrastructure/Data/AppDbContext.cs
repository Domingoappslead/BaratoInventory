using BaratoInventory.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace BaratoInventory.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Category).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Quantity).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired(false);
        });

        // Seed data
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Laptop", Category = "Electronics", Price = 999.99m, Quantity = 10, CreatedAt = DateTime.UtcNow },
            new Product { Id = 2, Name = "Mouse", Category = "Electronics", Price = 25.50m, Quantity = 50, CreatedAt = DateTime.UtcNow },
            new Product { Id = 3, Name = "Keyboard", Category = "Electronics", Price = 75.00m, Quantity = 30, CreatedAt = DateTime.UtcNow },
            new Product { Id = 4, Name = "Monitor", Category = "Electronics", Price = 299.99m, Quantity = 15, CreatedAt = DateTime.UtcNow },
            new Product { Id = 5, Name = "Desk Chair", Category = "Furniture", Price = 199.99m, Quantity = 20, CreatedAt = DateTime.UtcNow }
        );
    }


}
