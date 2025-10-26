using DataAccessLayer;
using EntityLayer;
using EntityLayer.DTOs;
using EntityLayer.DTOs.Category;
using EntityLayer.Enums;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Services
{
    public class CategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CategoryResponseDto>> GetAllCategoriesAsync(int? status = null)
        {
            var query = _context.Categories.AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(c => c.Status == status.Value);
            }

            var categories = await query.ToListAsync();

            return categories.Select(c => new CategoryResponseDto
            {
                Id = c.Id,
                Name = c.Name,
                // Description kaldırıldı
                Status = c.Status,
                StatusText = StatusHelper.GetCustomerStatusText(c.Status)
            }).ToList();
        }

        public async Task<CategoryResponseDto> CreateCategoryAsync(CategoryCreateDto categoryDto)
        {
            var category = new Category
            {
                Name = categoryDto.Name,
                // Description kaldırıldı
                Status = categoryDto.Status
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
                // Description kaldırıldı
                Status = category.Status,
                StatusText = StatusHelper.GetCustomerStatusText(category.Status)
            };
        }

        public async Task<bool> UpdateCategoryAsync(CategoryUpdateDto categoryDto)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == categoryDto.Id);
            if (category == null) return false;

            category.Name = categoryDto.Name;
            // Description kaldırıldı
            category.Status = categoryDto.Status;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return false;

            category.Status = 0; // Passive
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<CategoryResponseDto?> GetCategoryByIdAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return null;

            return new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
                // Description kaldırıldı
                Status = category.Status,
                StatusText = StatusHelper.GetCustomerStatusText(category.Status)
            };
        }
    }
}