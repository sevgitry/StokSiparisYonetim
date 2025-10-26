using DataAccessLayer;
using EntityLayer;
using EntityLayer.DTOs;
using EntityLayer.DTOs.User;
using EntityLayer.Enums;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Services
{
    public class AuthService
    {
        private readonly ApplicationDbContext _context;

        public AuthService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Login(string email, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.Password == password && u.Status == 1);

            return user != null;
        }

        public async Task<UserResponseDto?> GetUserByEmailAsync(string email)
        {
            var user = await _context.Users
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Email == email && u.Status == 1);

            if (user == null) return null;

            return new UserResponseDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                RolId = user.RolId,
                RoleName = user.Rol?.Name ?? "Unknown",
                Status = user.Status,
                StatusText = StatusHelper.GetUserStatusText(user.Status),
                CreatedDate = user.CreatedDate
            };
        }

        public async Task<UserResponseDto?> GetUserByIdAsync(int id)
        {
            var user = await _context.Users
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return null;

            return new UserResponseDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                RolId = user.RolId,
                RoleName = user.Rol?.Name ?? "Unknown",
                Status = user.Status,
                StatusText = StatusHelper.GetUserStatusText(user.Status),
                CreatedDate = user.CreatedDate
            };
        }

        public async Task<List<UserResponseDto>> GetAllUsersAsync()
        {
            var users = await _context.Users
                .Include(u => u.Rol)
                .Where(u => u.Status == 1)
                .ToListAsync();

            return users.Select(u => new UserResponseDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                RolId = u.RolId,
                RoleName = u.Rol?.Name ?? "Unknown",
                Status = u.Status,
                StatusText = StatusHelper.GetUserStatusText(u.Status),
                CreatedDate = u.CreatedDate
            }).ToList();
        }
    }
}