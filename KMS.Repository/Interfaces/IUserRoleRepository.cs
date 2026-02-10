using KMS.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Repository.Interfaces
{
    public interface IUserRoleRepository
    {
        Task<IEnumerable<KmsRole>> GetAllRolesAsync();
        Task<KmsRole?> GetRoleByIdAsync(int roleId);
        Task<KmsRole?> GetRoleByNameAsync(string roleName);
        Task<KmsUserRole?> GetUserRoleAsync(int userId);
        Task<string?> GetUserRoleNameAsync(int userId);
        Task<bool> AssignRoleToUserAsync(int userId, int roleId, int? assignedBy = null);
        Task<bool> UpdateUserRoleAsync(int userId, int newRoleId, int? updatedBy = null);
        Task<bool> RemoveRoleFromUserAsync(int userId);
        Task<bool> UserHasRoleAsync(int userId, int roleId);
        Task<bool> UserHasAnyRoleAsync(int userId);
        Task<IEnumerable<KmsUser>> GetUsersByRoleAsync(int roleId);
        Task<IEnumerable<KmsUser>> GetUsersByRoleNameAsync(string roleName);
        Task<IEnumerable<KmsUser>> GetUsersWithoutRoleAsync();
        Task<IEnumerable<KmsUser>> GetActiveUsersWithoutRoleAsync();
        Task<int> CountUsersByRoleAsync(int roleId);
        Task<int> CountUsersWithoutRoleAsync();
        Task<bool> ActivateUserAsync(int userId, int activatedBy);
        Task<bool> DeactivateUserAsync(int userId, int deactivatedBy);
        Task<IEnumerable<KmsUser>> GetPendingUsersAsync();
        Task<IEnumerable<KmsUser>> GetActiveUsersAsync();
        Task<IEnumerable<KmsUser>> GetInactiveUsersAsync();
        Task<int> CountPendingUsersAsync();
    }
}
