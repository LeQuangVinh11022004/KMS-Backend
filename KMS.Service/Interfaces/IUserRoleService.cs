using KMS.Service.DTOs;

namespace KMS.Service.Interfaces
{
    public interface IUserRoleService
    {
        // Roles
        Task<BaseResponseDTO> GetAllRolesAsync();
        Task<BaseResponseDTO> GetRoleByIdAsync(int roleId);

        // User Roles
        Task<BaseResponseDTO> GetUserRolesAsync(int userId);
        Task<BaseResponseDTO> AssignRoleToUserAsync(int userId, AssignRoleRequestDTO request, int assignedBy);
        Task<BaseResponseDTO> AssignRoleByNameAsync(int userId, AssignRoleByNameRequestDTO request, int assignedBy);
        Task<BaseResponseDTO> RemoveRoleFromUserAsync(int userId, int roleId);

        // Get users by role
        Task<BaseResponseDTO> GetUsersByRoleAsync(int roleId);
        Task<BaseResponseDTO> GetUsersByRoleNameAsync(string roleName);

        // Get all users with their roles
        Task<BaseResponseDTO> GetAllUsersWithRolesAsync();
    }
}