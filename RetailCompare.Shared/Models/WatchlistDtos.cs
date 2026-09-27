namespace RetailCompare.Shared.models;

public class WatchlistRequestDto
{
    public int ProductId { get; set; }
    public decimal TargetPrice { get; set; }
}

public class WatchlistItemDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string StoreName { get; set; } = string.Empty;
    public decimal CurrentPrice { get; set; }
    public decimal TargetPrice { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime AddedAt { get; set; }
}