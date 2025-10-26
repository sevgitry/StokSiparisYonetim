using DataAccessLayer;
using EntityLayer;
using EntityLayer.DTOs.Customer;
using EntityLayer.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class CustomerService
    {
        private readonly ApplicationDbContext _context;

        public CustomerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CustomerResponseDto>> GetAllCustomersAsync(int? status = null)
        {
            var query = _context.Customers.AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(c => c.Status == status.Value);
            }

            var customers = await query.ToListAsync();

            return customers.Select(c => new CustomerResponseDto
            {
                Id = c.Id,
                Name = c.Name,
                TaxNo = c.TaxNo,
                TaxAdministration = c.TaxAdministration,
                Status = c.Status,
                StatusText = StatusHelper.GetCustomerStatusText(c.Status), // Burada
                RowVersion = c.RowVersion
            }).ToList();
        }

        // Diğer metodlar benzer şekilde güncellenecek...
    }
}
