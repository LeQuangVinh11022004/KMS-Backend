using Microsoft.EntityFrameworkCore;
using KMS.Repository.Data;
using KMS.Repository.Entities;
using KMS.Repository.Interfaces;

namespace KMS.Repository.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<KmsUser?> GetByUsernameAsync(string username)
        {
            return await _context.KmsUsers
                .Include(u => u.KmsUserRoleUsers)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<KmsUser?> GetByIdAsync(int userId)
        {
            return await _context.KmsUsers
                .Include(u => u.KmsUserRoleUsers)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<KmsUser?> GetByEmailAsync(string email)
        {
            return await _context.KmsUsers
                .Include(u => u.KmsUserRoleUsers)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(int userId)
        {
            return await _context.KmsUserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.Role.RoleName)
                .ToListAsync();
        }

        public async Task<KmsUser> CreateAsync(KmsUser user)
        {
            _context.KmsUsers.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task UpdateAsync(KmsUser user)
        {
            user.UpdatedAt = DateTime.Now;
            _context.KmsUsers.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateLastLoginAsync(int userId)
        {
            var user = await _context.KmsUsers.FindAsync(userId);
            if (user != null)
            {
                user.LastLoginAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _context.KmsUsers
                .AnyAsync(u => u.Username == username);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.KmsUsers
                .AnyAsync(u => u.Email == email);
        }
    }
}