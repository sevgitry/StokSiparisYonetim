using System.ComponentModel.DataAnnotations;

namespace EntityLayer.DTOs.Category
{
    public class CategoryUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        // Description kaldırıldı
        // public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0, 1, ErrorMessage = "Geçersiz durum")]
        public int Status { get; set; }
    }
}