using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.DTOs.Order
{
    public class OrderUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [Range(1, 3, ErrorMessage = "Geçersiz durum")]
        public int Status { get; set; }

        public byte[] RowVersion { get; set; }
    }
}
