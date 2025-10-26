using System.ComponentModel.DataAnnotations;

namespace EntityLayer.DTOs.Category
{
    public class CategoryCreateDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        // Description kaldırıldı veya isteğe bağlı bırakıldı
        // public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0, 1, ErrorMessage = "Geçersiz durum")]
        public int Status { get; set; } = 1;
    }
}