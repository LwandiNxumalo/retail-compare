using System;
using System.Collections.Generic;
using System.Text;

namespace RetailCompare.Shared.models
{
    public class WatchlistRequest
    {
        public string UserId { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public decimal TargetPrice { get; set; }
    }
}
