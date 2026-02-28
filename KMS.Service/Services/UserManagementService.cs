using KMS.Repository.Interfaces;
using KMS.Service.DTOs;
using KMS.Service.Interfaces;
using KMS.Service.Services;
using KMS.Repository.Interfaces;
using KMS.Service.Interfaces;
using KMS.Service.DTOs.Role;
using KMS.Service.DTOs.User;

namespace KMS.Service.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRoleRepository _userRoleRepository;

        public UserManagementService(IUnitOfWork unitOfWork, IUserRoleRepository userRoleRepository)
        {
            _unitOfWork = unitOfWork;
            _userRoleRepository = userRoleRepository;
        }

        public async Task<BaseResponseDTO> ApproveUserAsync(int userId, int approvedBy)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return new BaseResponseDTO
                    {
                        Success = false,
                        Message = "User not found"
                    };
                }

                if (user.IsActive == true)
                {
                    return new BaseResponseDTO
                    {
                        Success = false,
                        Message = "User is already active"
                    };
                }

                // Activate user
                var success = await _unitOfWork.Users.ActivateUserAsync(userId, approvedBy);
                if (!success)
                {
                    return new BaseResponseDTO
                    {
                        Success = false,
                        Message = "Failed to activate user"
                    };
                }

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"User '{user.Username}' has been approved and activated successfully"
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }

        // ============================================================
        // REJECT USER (Giữ inactive hoặc xóa)
        // ============================================================

        public async Task<BaseResponseDTO> RejectUserAsync(int userId, int rejectedBy)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return new BaseResponseDTO
                    {
                        Success = false,
                        Message = "User not found"
                    };
                }

                // Option 1: Giữ inactive (để có thể review lại sau)
                var success = await _unitOfWork.Users.DeactivateUserAsync(userId, rejectedBy);

                // Option 2: Hoặc có thể xóa luôn (tùy requirement)
                // await _context.Users.Remove(user);

                if (!success)
                {
                    return new BaseResponseDTO
                    {
                        Success = false,
                        Message = "Failed to reject user"
                    };
                }

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"User '{user.Username}' has been rejected"
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }

        // ============================================================
        // DEACTIVATE USER (Cho users đang active)
        // ============================================================

        public async Task<BaseResponseDTO> DeactivateUserAsync(int userId, int deactivatedBy)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return new BaseResponseDTO
                    {
                        Success = false,
                        Message = "User not found"
                    };
                }

                if (user.IsActive != true)
                {
                    return new BaseResponseDTO
                    {
                        Success = false,
                        Message = "User is already inactive"
                    };
                }

                var success = await _unitOfWork.Users.DeactivateUserAsync(userId, deactivatedBy);
                if (!success)
                {
                    return new BaseResponseDTO
                    {
                        Success = false,
                        Message = "Failed to deactivate user"
                    };
                }

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"User '{user.Username}' has been deactivated"
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }

        // ============================================================
        // GET USERS BY STATUS
        // ============================================================

        public async Task<BaseResponseDTO> GetPendingUsersAsync()
        {
            try
            {
                var users = await _unitOfWork.Users.GetPendingUsersAsync();
                var userDTOs = new List<object>();

                foreach (var user in users)
                {
                    var roleName = await _userRoleRepository.GetUserRoleNameAsync(user.UserId);

                    userDTOs.Add(new
                    {
                        UserId = user.UserId,
                        Username = user.Username,
                        FullName = user.FullName,
                        Email = user.Email,
                        Phone = user.Phone,
                        RoleName = roleName ?? "No Role",
                        CreatedAt = user.CreatedAt
                    });
                }

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"Found {userDTOs.Count} pending users",
                    Data = userDTOs
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }

        public async Task<BaseResponseDTO> GetActiveUsersAsync()
        {
            try
            {
                var users = await _unitOfWork.Users.GetActiveUsersAsync();
                var userDTOs = users.Select(u => new UserDTO
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    FullName = u.FullName,
                    Email = u.Email,
                    Phone = u.Phone,
                    IsActive = true
                }).ToList();

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"Found {userDTOs.Count} active users",
                    Data = userDTOs
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }

        public async Task<BaseResponseDTO> GetInactiveUsersAsync()
        {
            try
            {
                var users = await _unitOfWork.Users.GetInactiveUsersAsync();
                var userDTOs = users.Select(u => new UserDTO
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    FullName = u.FullName,
                    Email = u.Email,
                    Phone = u.Phone,
                    IsActive = false
                }).ToList();

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"Found {userDTOs.Count} inactive users",
                    Data = userDTOs
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }

        // ============================================================
        // STATISTICS
        // ============================================================

        public async Task<BaseResponseDTO> GetUserStatusStatisticsAsync()
        {
            try
            {
                var pendingCount = await _unitOfWork.Users.CountPendingUsersAsync();
                var activeUsers = await _unitOfWork.Users.GetActiveUsersAsync();
                var inactiveUsers = await _unitOfWork.Users.GetInactiveUsersAsync();

                var result = new
                {
                    TotalUsers = activeUsers.Count() + inactiveUsers.Count(),
                    ActiveUsers = activeUsers.Count(),
                    InactiveUsers = inactiveUsers.Count(),
                    PendingApproval = pendingCount
                };

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = "User status statistics retrieved successfully",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }
    }
}