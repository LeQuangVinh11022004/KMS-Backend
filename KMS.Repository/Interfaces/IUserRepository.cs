using KMS.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<KmsUser?> GetByUsernameAsync(string username);
        Task<KmsUser?> GetByIdAsync(int userId);
        Task<KmsUser?> GetByEmailAsync(string email);
        Task<IEnumerable<string>> GetUserRolesAsync(int userId);
        Task<KmsUser> CreateAsync(KmsUser user);
        Task UpdateAsync(KmsUser user);
        Task UpdateLastLoginAsync(int userId);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> ActivateUserAsync(int userId, int activatedBy);
        Task<bool> DeactivateUserAsync(int userId, int deactivatedBy);
        Task<IEnumerable<KmsUser>> GetPendingUsersAsync();
        Task<IEnumerable<KmsUser>> GetActiveUsersAsync();
        Task<IEnumerable<KmsUser>> GetInactiveUsersAsync();
        Task<int> CountPendingUsersAsync();
    }
}
