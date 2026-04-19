using KMS.Service.DTOs.Role;
using KMS.Service.DTOs.User;

namespace KMS.Service.Interfaces
{
    public interface IUserManagementService
    {
        // Activation
        Task<BaseResponseDTO> ApproveUserAsync(int userId, int approvedBy);
        Task<BaseResponseDTO> RejectUserAsync(int userId, int rejectedBy);
        Task<BaseResponseDTO> DeactivateUserAsync(int userId, int deactivatedBy);

        // Get Users by Status
        Task<BaseResponseDTO> GetPendingUsersAsync();
        Task<BaseResponseDTO> GetActiveUsersAsync();
        Task<BaseResponseDTO> GetInactiveUsersAsync();

        // Statistics
        Task<BaseResponseDTO> GetUserStatusStatisticsAsync();

        Task<BaseResponseDTO> UpdateUserAsync(int userId, UpdateUserDTO dto);
    }
}