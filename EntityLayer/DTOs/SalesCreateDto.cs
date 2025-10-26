using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.DTOs
{
    public class SalesCreateDto
    {
        public int ProductId { get; set; }
        public int SaleAmount { get; set; }
        public int CustomerId { get; set; }
    }
}
