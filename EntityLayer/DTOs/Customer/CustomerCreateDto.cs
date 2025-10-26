using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.DTOs.Customer
{
    public class CustomerCreateDto
    {
        [Required]
        public string Name { get; set; }

        public string TaxNo { get; set; }
        public string TaxAdministration { get; set; }

        [Required]
        [Range(0, 1, ErrorMessage = "Geçersiz durum")]
        public int Status { get; set; } = 1; // 0: Passive, 1: Active
    }
}
