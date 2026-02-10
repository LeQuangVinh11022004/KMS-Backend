using Microsoft.EntityFrameworkCore;
using KMS.Repository.Data;
using KMS.Repository.Entities;
using KMS.Repository.Interfaces;

namespace KMS.Repository.Repositories
{
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRoleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<KmsRole>> GetAllRolesAsync()
        {
            return await _context.KmsRoles
                .OrderBy(r => r.RoleName)
                .ToListAsync();
        }

        public async Task<KmsRole?> GetRoleByIdAsync(int roleId)
        {
            return await _context.KmsRoles
                .FirstOrDefaultAsync(r => r.RoleId == roleId);
        }

        public async Task<KmsRole?> GetRoleByNameAsync(string roleName)
        {
            return await _context.KmsRoles
                .FirstOrDefaultAsync(r => r.RoleName == roleName);
        }

        public async Task<KmsUserRole?> GetUserRoleAsync(int userId)
        {
            return await _context.KmsUserRoles
                .Include(ur => ur.Role)
                .Include(ur => ur.User)
                .FirstOrDefaultAsync(ur => ur.UserId == userId);
        }

        public async Task<string?> GetUserRoleNameAsync(int userId)
        {
            var userRole = await _context.KmsUserRoles
                .Include(ur => ur.Role)
                .FirstOrDefaultAsync(ur => ur.UserId == userId);

            return userRole?.Role.RoleName;
        }

        public async Task<bool> AssignRoleToUserAsync(int userId, int roleId, int? assignedBy = null)
        {
            var userExists = await _context.KmsUsers.AnyAsync(u => u.UserId == userId);
            if (!userExists) return false;

            var roleExists = await _context.KmsRoles.AnyAsync(r => r.RoleId == roleId);
            if (!roleExists) return false;

            var existingRoles = await _context.KmsUserRoles
                .Where(ur => ur.UserId == userId)
                .ToListAsync();

            if (existingRoles.Any())
            {
                _context.KmsUserRoles.RemoveRange(existingRoles);
            }

            var userRole = new KmsUserRole
            {
                UserId = userId,
                RoleId = roleId,
                AssignedBy = assignedBy,
                AssignedAt = DateTime.Now
            };

            _context.KmsUserRoles.Add(userRole);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateUserRoleAsync(int userId, int newRoleId, int? updatedBy = null)
        {
            return await AssignRoleToUserAsync(userId, newRoleId, updatedBy);
        }

        public async Task<bool> RemoveRoleFromUserAsync(int userId)
        {
            var userRoles = await _context.KmsUserRoles
                .Where(ur => ur.UserId == userId)
                .ToListAsync();

            if (!userRoles.Any()) return false;

            _context.KmsUserRoles.RemoveRange(userRoles);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UserHasRoleAsync(int userId, int roleId)
        {
            return await _context.KmsUserRoles
                .AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
        }

        public async Task<bool> UserHasAnyRoleAsync(int userId)
        {
            return await _context.KmsUserRoles
                .AnyAsync(ur => ur.UserId == userId);
        }

        public async Task<IEnumerable<KmsUser>> GetUsersByRoleAsync(int roleId)
        {
            return await _context.KmsUserRoles
                .Where(ur => ur.RoleId == roleId)
                .Select(ur => ur.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<KmsUser>> GetUsersByRoleNameAsync(string roleName)
        {
            return await _context.KmsUserRoles
                .Where(ur => ur.Role.RoleName == roleName)
                .Select(ur => ur.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<KmsUser>> GetUsersWithoutRoleAsync()
        {
            // Users không có role nào
            var usersWithRole = await _context.KmsUserRoles
                .Select(ur => ur.UserId)
                .Distinct()
                .ToListAsync();

            return await _context.KmsUsers
                .Where(u => !usersWithRole.Contains(u.UserId))
                .ToListAsync();
        }

        public async Task<IEnumerable<KmsUser>> GetActiveUsersWithoutRoleAsync()
        {
            // Users active và không có role
            var usersWithRole = await _context.KmsUserRoles
                .Select(ur => ur.UserId)
                .Distinct()
                .ToListAsync();

            return await _context.KmsUsers
                .Where(u => !usersWithRole.Contains(u.UserId) && u.IsActive == true)
                .ToListAsync();
        }

        public async Task<int> CountUsersByRoleAsync(int roleId)
        {
            return await _context.KmsUserRoles
                .CountAsync(ur => ur.RoleId == roleId);
        }

        public async Task<int> CountUsersWithoutRoleAsync()
        {
            var usersWithRole = await _context.KmsUserRoles
                .Select(ur => ur.UserId)
                .Distinct()
                .CountAsync();

            var totalUsers = await _context.KmsUsers.CountAsync();
            return totalUsers - usersWithRole;
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