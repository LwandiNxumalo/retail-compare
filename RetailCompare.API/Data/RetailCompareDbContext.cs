using Microsoft.EntityFrameworkCore;
using RetailCompare.Shared.models;

namespace RetailCompare.API.Data
{
    public class RetailCompareDbContext : DbContext
    {
        public RetailCompareDbContext(DbContextOptions<RetailCompareDbContext> options) : base(options) { }

        public DbSet<ProductDto> Products => Set<ProductDto>();
        public DbSet<PriceHistoryDto> PriceHistories => Set<PriceHistoryDto>();
        public DbSet<WatchlistRequest> Watchlists => Set<WatchlistRequest>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure primary key for PriceHistoryDto if treating as entity
            modelBuilder.Entity<PriceHistoryDto>().HasKey(p => new { p.StoreName, p.Timestamp });

            // Configure primary key for WatchlistRequest
            modelBuilder.Entity<WatchlistRequest>().HasKey(w => new { w.UserId, w.ProductId });
        }
    }
}
