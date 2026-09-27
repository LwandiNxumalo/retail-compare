namespace RetailCompare.API.Data;

public class PriceHistory
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public string StoreName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime Timestamp { get; set; }
    public bool IsOnSale { get; set; }
}