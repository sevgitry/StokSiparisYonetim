using System;

namespace EntityLayer.DTOs
{
    public class CartItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public int MaxStock { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public decimal TotalPrice => Quantity * UnitPrice;
    }
}