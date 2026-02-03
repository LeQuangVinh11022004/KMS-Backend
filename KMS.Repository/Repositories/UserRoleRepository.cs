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

        // ============================================================
        // GET ROLES
        // ============================================================

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

        // ============================================================
        // GET USER ROLES
        // ============================================================

        public async Task<IEnumerable<KmsUserRole>> GetUserRolesAsync(int userId)
        {
            return await _context.KmsUserRoles
                .Include(ur => ur.Role)
                .Include(ur => ur.User)
                .Where(ur => ur.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetUserRoleNamesAsync(int userId)
        {
            return await _context.KmsUserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.Role.RoleName)
                .ToListAsync();
        }

        // ============================================================
        // ASSIGN/REMOVE ROLES
        // ============================================================

        public async Task<bool> AssignRoleToUserAsync(int userId, int roleId, int? assignedBy = null)
        {
            // Check if user exists
            var userExists = await _context.KmsUsers.AnyAsync(u => u.UserId == userId);
            if (!userExists) return false;

            // Check if role exists
            var roleExists = await _context.KmsRoles.AnyAsync(r => r.RoleId == roleId);
            if (!roleExists) return false;

            // Check if user already has this role
            var alreadyHasRole = await UserHasRoleAsync(userId, roleId);
            if (alreadyHasRole) return false;

            // Assign role
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

        public async Task<bool> RemoveRoleFromUserAsync(int userId, int roleId)
        {
            var userRole = await _context.KmsUserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

            if (userRole == null) return false;

            _context.KmsUserRoles.Remove(userRole);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UserHasRoleAsync(int userId, int roleId)
        {
            return await _context.KmsUserRoles
                .AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
        }

        // ============================================================
        // GET USERS BY ROLE
        // ============================================================

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
    }
}