using Microsoft.EntityFrameworkCore;
using ServiceRepoApi.Models;

namespace ServiceRepoApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
            .HasIndex(p => p.Name)
            .IsUnique();

        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Keyboard", Description = "Mechanical keyboard", Price = 79.99m, Stock = 25, CreatedAt = new DateTime(2026, 1, 1) },
            new Product { Id = 2, Name = "Mouse", Description = "Wireless mouse", Price = 29.50m, Stock = 60, CreatedAt = new DateTime(2026, 1, 1) }
        );
    }
}
