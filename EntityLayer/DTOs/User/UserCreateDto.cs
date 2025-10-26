using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.DTOs.User
{
    public class UserCreateDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public int RolId { get; set; }

        [Required]
        [Range(0, 1, ErrorMessage = "Geçersiz durum")]
        public int Status { get; set; } = 1; // 0: Passive, 1: Active
    }
}
