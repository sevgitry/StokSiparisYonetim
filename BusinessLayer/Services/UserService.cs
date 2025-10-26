using DataAccessLayer;
using EntityLayer;
using EntityLayer.DTOs.User;
using EntityLayer.Enums;
using Microsoft.EntityFrameworkCore;
using EntityLayer.DTOs;

namespace BusinessLayer.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserResponseDto>> GetAllUsersAsync(int? status = null)
        {
            var query = _context.Users
                .Include(u => u.Rol)
                .AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(u => u.Status == status.Value);
            }

            var users = await query.ToListAsync();

            return users.Select(u => new UserResponseDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                RolId = u.RolId,
                RoleName = u.Rol?.Name,
                Status = u.Status,
                StatusText = StatusHelper.GetUserStatusText(u.Status),
                CreatedDate = u.CreatedDate
            }).ToList();
        }

        public async Task<UserResponseDto> CreateUserAsync(UserCreateDto userDto)
        {
            // Email kontrolü
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == userDto.Email);
            if (existingUser != null)
            {
                throw new Exception("Bu email adresi zaten kullanılıyor");
            }

            var user = new User
            {
                Name = userDto.Name,
                Email = userDto.Email,
                Password = userDto.Password,
                RolId = userDto.RolId,
                Status = userDto.Status,
                CreatedDate = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var createdUser = await GetUserByIdAsync(user.Id);
            if (createdUser == null)
            {
                throw new Exception("Kullanıcı oluşturuldu ancak getirilemedi");
            }

            return createdUser;
        }

        public async Task<bool> UpdateUserAsync(int id, UserUpdateDto userDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userDto.Id);
            if (user == null) return false;

            // Email kontrolü (başka kullanıcıda var mı?)
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == userDto.Email && u.Id != userDto.Id);
            if (existingUser != null)
            {
                throw new Exception("Bu email adresi başka bir kullanıcı tarafından kullanılıyor");
            }

            user.Name = userDto.Name;
            user.Email = userDto.Email;
            user.RolId = userDto.RolId;
            user.Status = userDto.Status;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            user.Status = 0; // Passive
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<UserResponseDto> GetUserByIdAsync(int id)
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
                RoleName = user.Rol?.Name,
                Status = user.Status,
                StatusText = StatusHelper.GetUserStatusText(user.Status),
                CreatedDate = user.CreatedDate
            };
        }

        public async Task<bool> ChangeUserPasswordAsync(int userId, string newPassword)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            user.Password = newPassword; // Production'da hash'lenmeli
            await _context.SaveChangesAsync();
            return true;
        }
    }
}