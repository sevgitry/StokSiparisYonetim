using EntityLayer.DTOs.Order;
using System.ComponentModel.DataAnnotations;

namespace EntityLayer.DTOs
{
    public class OrderCreateDto
    {
        [Required]
        public int CustomerId { get; set; }

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required]
        public List<OrderItemCreateDto> OrderItems { get; set; } = new();
    }

   
}