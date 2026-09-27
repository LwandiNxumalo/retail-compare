namespace RetailCompare.API.Data;

public class WatchlistItem
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public decimal TargetPrice { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}