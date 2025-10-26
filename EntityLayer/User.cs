using System;
using System.ComponentModel.DataAnnotations;

namespace EntityLayer
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int RolId { get; set; }
        public int Status { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public byte[] RowVersion { get; set; } // YENİ EKLENDİ

        // Navigation property
        public Rol Rol { get; set; }
    }
}