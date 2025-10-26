using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.DTOs.User
{
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int RolId { get; set; }
        public string RoleName { get; set; }
        public int Status { get; set; }
        public string StatusText { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
