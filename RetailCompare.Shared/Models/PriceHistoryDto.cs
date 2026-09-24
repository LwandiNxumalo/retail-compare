using System;
using System.Collections.Generic;
using System.Text;

namespace RetailCompare.Shared.models
{
    public class PriceHistoryDto
    {
        public string StoreName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsOnSale { get; set; }
    }
}
