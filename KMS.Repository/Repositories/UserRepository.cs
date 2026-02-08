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

        public async Task<bool> ActivateUserAsync(int userId, int activatedBy)
        {
            var user = await _context.KmsUsers.FindAsync(userId);
            if (user == null) return false;

            user.IsActive = true;
            user.UpdatedAt = DateTime.Now;
            // Note: Có thể log vào AuditLogs với activatedBy

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeactivateUserAsync(int userId, int deactivatedBy)
        {
            var user = await _context.KmsUsers.FindAsync(userId);
            if (user == null) return false;

            user.IsActive = false;
            user.UpdatedAt = DateTime.Now;
            // Note: Có thể log vào AuditLogs với deactivatedBy

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<KmsUser>> GetPendingUsersAsync()
        {
            // Users IsActive = false hoặc null
            return await _context.KmsUsers
                .Where(u => u.IsActive != true)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<KmsUser>> GetActiveUsersAsync()
        {
            return await _context.KmsUsers
                .Where(u => u.IsActive == true)
                .OrderBy(u => u.Username)
                .ToListAsync();
        }

        public async Task<IEnumerable<KmsUser>> GetInactiveUsersAsync()
        {
            // Tương đương GetPendingUsers, nhưng có thể filter thêm
            return await _context.KmsUsers
                .Where(u => u.IsActive == false)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> CountPendingUsersAsync()
        {
            return await _context.KmsUsers
                .CountAsync(u => u.IsActive != true);
        }
    }
}