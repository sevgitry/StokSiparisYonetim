using System.ComponentModel.DataAnnotations;

namespace EntityLayer
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TaxNo { get; set; }
        public string TaxAdministration { get; set; }
        public int Status { get; set; } = 1;

        public byte[] RowVersion { get; set; }

        // Navigation property
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}