using System.ComponentModel.DataAnnotations;

namespace EntityLayer
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string ProductName { get; set; } = string.Empty;

        public float SellPrice { get; set; }
        public int Amount { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int SellerId { get; set; }
        public int CategoryId { get; set; }
        public float PurchasePrice { get; set; }
        public int Status { get; set; } = 1;

        [Timestamp]
        public byte[] RowVersion { get; set; }

        // Navigation properties
        public Category Category { get; set; }
        public User Seller { get; set; }
    }
}