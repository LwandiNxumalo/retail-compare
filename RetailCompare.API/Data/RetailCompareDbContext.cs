using Microsoft.EntityFrameworkCore;
using RetailCompare.Shared.models;

namespace RetailCompare.API.Data;

public class RetailCompareDbContext : DbContext
{
    public RetailCompareDbContext(DbContextOptions<RetailCompareDbContext> options) : base(options) { }

    public DbSet<ProductDto> Products => Set<ProductDto>();
    public DbSet<PriceHistoryDto> PriceHistories => Set<PriceHistoryDto>();
    public DbSet<WatchlistRequest> Watchlists => Set<WatchlistRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Composite Primary Keys
        modelBuilder.Entity<PriceHistoryDto>().HasKey(p => new { p.StoreName, p.Timestamp });
        modelBuilder.Entity<WatchlistRequest>().HasKey(w => new { w.UserId, w.ProductId });

        // --- Seed Sample Products ---
        modelBuilder.Entity<ProductDto>().HasData(
            new ProductDto
            {
                Id = 1,
                Name = "Full Cream Milk 2L",
                Category = "Groceries",
                ImageUrl = "https://via.placeholder.com/150",
                CurrentLowestPrice = 32.99m
            },
            new ProductDto
            {
                Id = 2,
                Name = "White Bread 700g",
                Category = "Bakery",
                ImageUrl = "https://via.placeholder.com/150",
                CurrentLowestPrice = 16.50m
            },
            new ProductDto
            {
                Id = 3,
                Name = "Instant Coffee 200g",
                Category = "Pantry",
                ImageUrl = "https://via.placeholder.com/150",
                CurrentLowestPrice = 119.99m
            }
        );

        // --- Seed Sample Price Histories ---
        modelBuilder.Entity<PriceHistoryDto>().HasData(
            new PriceHistoryDto
            {
                StoreName = "SuperStore",
                Price = 32.99m,
                Timestamp = DateTime.SpecifyKind(new DateTime(2026, 9, 20), DateTimeKind.Utc),
                IsOnSale = true
            },
            new PriceHistoryDto
            {
                StoreName = "ValueMart",
                Price = 35.50m,
                Timestamp = DateTime.SpecifyKind(new DateTime(2026, 9, 21), DateTimeKind.Utc),
                IsOnSale = false
            }
        );
    }
}