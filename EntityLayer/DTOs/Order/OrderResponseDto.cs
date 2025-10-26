using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.DTOs.Order
{
    public class OrderResponseDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerTaxNo { get; set; }
        public DateTime OrderDate { get; set; }
        public int Status { get; set; }
        public string StatusText { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderItemResponseDto> OrderItems { get; set; } = new();
        public byte[] RowVersion { get; set; }
    }
}
