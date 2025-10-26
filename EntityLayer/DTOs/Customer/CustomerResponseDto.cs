using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.DTOs.Customer
{
    public class CustomerResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TaxNo { get; set; }
        public string TaxAdministration { get; set; }
        public int Status { get; set; }
        public string StatusText { get; set; }
        public byte[] RowVersion { get; set; }
    }
}
