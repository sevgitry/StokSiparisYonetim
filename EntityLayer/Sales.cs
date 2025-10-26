using System.ComponentModel.DataAnnotations;

namespace EntityLayer
{
    public class Sales
    {
        public int Id { get; set; }
        public int ProductId { get; set; }

        [Range(1, int.MaxValue)]
        public int SaleAmount { get; set; }

        public int CustomerId { get; set; }
        public int CategoryId { get; set; }
        public DateTime SaleDate { get; set; } = DateTime.Now;
        public int Status { get; set; } = 0; // 0: Draft, 1: Approved, 2: Cancelled

        [Timestamp]
        public byte[] RowVersion { get; set; }

        // Navigation properties
        public Product Product { get; set; }
        public Customer Customer { get; set; }
        public Category Category { get; set; }
    }
}