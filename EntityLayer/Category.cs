using System.ComponentModel.DataAnnotations;

namespace EntityLayer
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Status { get; set; } = 1;
        public int ParentCategoryId { get; set; }

        public byte[] RowVersion { get; set; }

        // Navigation property - Products eklendi
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}