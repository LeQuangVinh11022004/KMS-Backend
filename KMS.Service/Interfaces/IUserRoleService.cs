using KMS.Service.DTOs;

namespace KMS.Service.Interfaces
{
    public interface IUserRoleService
    {
        // Roles
        Task<BaseResponseDTO> GetAllRolesAsync();
        Task<BaseResponseDTO> GetRoleByIdAsync(int roleId);

        // User Role
        Task<BaseResponseDTO> GetUserRoleAsync(int userId);

        // Assign/Update/Remove
        Task<BaseResponseDTO> AssignRoleToUserAsync(int userId, AssignRoleRequestDTO request, int assignedBy);
        Task<BaseResponseDTO> AssignRoleByNameAsync(int userId, AssignRoleByNameRequestDTO request, int assignedBy);
        Task<BaseResponseDTO> UpdateUserRoleAsync(int userId, AssignRoleRequestDTO request, int updatedBy);
        Task<BaseResponseDTO> RemoveRoleFromUserAsync(int userId);

        // Get users by role
        Task<BaseResponseDTO> GetUsersByRoleAsync(int roleId);
        Task<BaseResponseDTO> GetUsersByRoleNameAsync(string roleName);

        // Filter
        Task<BaseResponseDTO> GetUsersWithoutRoleAsync();
        Task<BaseResponseDTO> GetActiveUsersWithoutRoleAsync();

        // Statistics
        Task<BaseResponseDTO> GetRoleStatisticsAsync();
    }
}