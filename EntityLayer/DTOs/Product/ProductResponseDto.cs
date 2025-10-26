namespace EntityLayer.DTOs
{
    public class ProductResponseDto
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public float SellPrice { get; set; }
        public float PurchasePrice { get; set; }
        public int Amount { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int SellerId { get; set; }
        public string SellerName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public int Status { get; set; }
        public string StatusText { get; set; } = string.Empty;
    }
}