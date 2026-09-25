using System;
using System.Collections.Generic;
using System.Text;

namespace RetailCompare.Shared.models
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public decimal CurrentLowestPrice { get; set; }

        public string Description { get; set; } = string.Empty;
    }
}
