using System.ComponentModel.DataAnnotations;

namespace EntityLayer.DTOs
{
    public class ProductCreateDto
    {
        [Required]
        public string ProductName { get; set; } = string.Empty;

        [Required]
        public float SellPrice { get; set; }

        [Required]
        public float PurchasePrice { get; set; }

        [Required]
        public int Amount { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int SellerId { get; set; }

        [Required]
        [Range(0, 1, ErrorMessage = "Geçersiz durum")]
        public int Status { get; set; } = 1;
    }
}