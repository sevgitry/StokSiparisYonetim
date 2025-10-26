using System;
using System.ComponentModel.DataAnnotations;
namespace EntityLayer
{
    public class Rol
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<User> Users { get; set; }
    }
}