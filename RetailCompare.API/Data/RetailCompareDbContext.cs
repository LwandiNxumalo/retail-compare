using Microsoft.EntityFrameworkCore;

namespace RetailCompare.API.Data;

public class RetailCompareDbContext : DbContext
{
    public RetailCompareDbContext(DbContextOptions<RetailCompareDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<WatchlistItem> WatchlistItems => Set<WatchlistItem>();
    public DbSet<PriceHistory> PriceHistories => Set<PriceHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // --- Configure WatchlistItem ---
        modelBuilder.Entity<WatchlistItem>()
            .HasKey(w => w.Id);

        // Ensure a user cannot add the same product twice
        modelBuilder.Entity<WatchlistItem>()
            .HasIndex(w => new { w.UserId, w.ProductId })
            .IsUnique();

        modelBuilder.Entity<WatchlistItem>()
            .HasOne(w => w.Product)
            .WithMany()
            .HasForeignKey(w => w.ProductId);

        // --- Seed Sample Products ---
        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 1,
                Name = "Full Cream Milk 2L",
                Category = "Groceries",
                StoreName = "SuperStore",
                Price = 32.99m,
                ImageUrl = "https://via.placeholder.com/150",
                Description = "Fresh full cream milk"
            },
            new Product
            {
                Id = 2,
                Name = "White Bread 700g",
                Category = "Bakery",
                StoreName = "ValueMart",
                Price = 16.50m,
                ImageUrl = "https://via.placeholder.com/150",
                Description = "Sliced white bread"
            },
            new Product
            {
                Id = 3,
                Name = "Instant Coffee 200g",
                Category = "Pantry",
                StoreName = "SuperStore",
                Price = 119.99m,
                ImageUrl = "https://via.placeholder.com/150",
                Description = "Rich roasted instant coffee"
            }
        );

        // --- Seed Sample Price Histories ---
        modelBuilder.Entity<PriceHistory>().HasData(
            new PriceHistory
            {
                Id = 1,
                ProductId = 1,
                StoreName = "SuperStore",
                Price = 32.99m,
                Timestamp = DateTime.SpecifyKind(new DateTime(2026, 9, 20), DateTimeKind.Utc),
                IsOnSale = true
            },
            new PriceHistory
            {
                Id = 2,
                ProductId = 1,
                StoreName = "ValueMart",
                Price = 35.50m,
                Timestamp = DateTime.SpecifyKind(new DateTime(2026, 9, 21), DateTimeKind.Utc),
                IsOnSale = false
            }
        );
    }
}