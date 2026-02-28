using KMS.Repository.Interfaces;
using KMS.Service.DTOs;
using KMS.Service.DTOs.Role;
using KMS.Service.DTOs.User;
using KMS.Service.Interfaces;

namespace KMS.Service.Services
{
    public class UserRoleService : IUserRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRoleRepository _userRoleRepository;

        public UserRoleService(IUnitOfWork unitOfWork, IUserRoleRepository userRoleRepository)
        {
            _unitOfWork = unitOfWork;
            _userRoleRepository = userRoleRepository;
        }

        // ============================================================
        // ROLES
        // ============================================================

        public async Task<BaseResponseDTO> GetAllRolesAsync()
        {
            try
            {
                var roles = await _userRoleRepository.GetAllRolesAsync();
                var roleDTOs = roles.Select(r => new RoleDTO
                {
                    RoleId = r.RoleId,
                    RoleName = r.RoleName,
                    Description = r.Description
                }).ToList();

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = "Roles retrieved successfully",
                    Data = roleDTOs
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

        public async Task<BaseResponseDTO> GetRoleByIdAsync(int roleId)
        {
            try
            {
                var role = await _userRoleRepository.GetRoleByIdAsync(roleId);
                if (role == null)
                {
                    return new BaseResponseDTO
                    {
                        Success = false,
                        Message = "Role not found"
                    };
                }

                var roleDTO = new RoleDTO
                {
                    RoleId = role.RoleId,
                    RoleName = role.RoleName,
                    Description = role.Description
                };

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = "Role retrieved successfully",
                    Data = roleDTO
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
        // GET USER ROLE (SINGULAR)
        // ============================================================

        public async Task<BaseResponseDTO> GetUserRoleAsync(int userId)
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

                var userRole = await _userRoleRepository.GetUserRoleAsync(userId);

                RoleDTO? roleDTO = null;
                if (userRole != null)
                {
                    roleDTO = new RoleDTO
                    {
                        RoleId = userRole.Role.RoleId,
                        RoleName = userRole.Role.RoleName,
                        Description = userRole.Role.Description
                    };
                }

                var result = new
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    FullName = user.FullName,
                    Role = roleDTO,
                    HasRole = roleDTO != null
                };

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = "User role retrieved successfully",
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

        // ============================================================
        // ASSIGN ROLE
        // ============================================================

        public async Task<BaseResponseDTO> AssignRoleToUserAsync(int userId, AssignRoleRequestDTO request, int assignedBy)
        {
            try
            {
                var success = await _userRoleRepository.AssignRoleToUserAsync(
                    userId,
                    request.RoleId,
                    assignedBy
                );

                if (!success)
                {
                    return new BaseResponseDTO
                    {
                        Success = false,
                        Message = "Failed to assign role. User or role not found."
                    };
                }

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = "Role assigned successfully"
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

        public async Task<BaseResponseDTO> AssignRoleByNameAsync(int userId, AssignRoleByNameRequestDTO request, int assignedBy)
        {
            try
            {
                var role = await _userRoleRepository.GetRoleByNameAsync(request.RoleName);
                if (role == null)
                {
                    return new BaseResponseDTO
                    {
                        Success = false,
                        Message = $"Role '{request.RoleName}' not found"
                    };
                }

                var success = await _userRoleRepository.AssignRoleToUserAsync(
                    userId,
                    role.RoleId,
                    assignedBy
                );

                if (!success)
                {
                    return new BaseResponseDTO
                    {
                        Success = false,
                        Message = "Failed to assign role. User not found."
                    };
                }

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"Role '{request.RoleName}' assigned successfully"
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
        // UPDATE ROLE
        // ============================================================

        public async Task<BaseResponseDTO> UpdateUserRoleAsync(int userId, AssignRoleRequestDTO request, int updatedBy)
        {
            try
            {
                var success = await _userRoleRepository.UpdateUserRoleAsync(
                    userId,
                    request.RoleId,
                    updatedBy
                );

                if (!success)
                {
                    return new BaseResponseDTO
                    {
                        Success = false,
                        Message = "Failed to update role. User or role not found."
                    };
                }

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = "Role updated successfully"
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
        // REMOVE ROLE
        // ============================================================

        public async Task<BaseResponseDTO> RemoveRoleFromUserAsync(int userId)
        {
            try
            {
                var success = await _userRoleRepository.RemoveRoleFromUserAsync(userId);
                if (!success)
                {
                    return new BaseResponseDTO
                    {
                        Success = false,
                        Message = "Failed to remove role. User has no role."
                    };
                }

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = "Role removed successfully"
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
        // GET USERS BY ROLE
        // ============================================================

        public async Task<BaseResponseDTO> GetUsersByRoleAsync(int roleId)
        {
            try
            {
                var users = await _userRoleRepository.GetUsersByRoleAsync(roleId);
                var userDTOs = users.Select(u => new UserDTO
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    FullName = u.FullName,
                    Email = u.Email,
                    Phone = u.Phone,
                    IsActive = u.IsActive == true
                }).ToList();

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = "Users retrieved successfully",
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

        public async Task<BaseResponseDTO> GetUsersByRoleNameAsync(string roleName)
        {
            try
            {
                var users = await _userRoleRepository.GetUsersByRoleNameAsync(roleName);
                var userDTOs = users.Select(u => new UserDTO
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    FullName = u.FullName,
                    Email = u.Email,
                    Phone = u.Phone,
                    IsActive = u.IsActive == true
                }).ToList();

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"Users with role '{roleName}' retrieved successfully",
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
        // FILTER USERS
        // ============================================================

        public async Task<BaseResponseDTO> GetUsersWithoutRoleAsync()
        {
            try
            {
                var users = await _userRoleRepository.GetUsersWithoutRoleAsync();
                var userDTOs = users.Select(u => new UserDTO
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    FullName = u.FullName,
                    Email = u.Email,
                    Phone = u.Phone,
                    IsActive = u.IsActive == true,
                    Roles = new List<string>()
                }).ToList();

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"Found {userDTOs.Count} users without role",
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

        public async Task<BaseResponseDTO> GetActiveUsersWithoutRoleAsync()
        {
            try
            {
                var users = await _userRoleRepository.GetActiveUsersWithoutRoleAsync();
                var userDTOs = users.Select(u => new UserDTO
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    FullName = u.FullName,
                    Email = u.Email,
                    Phone = u.Phone,
                    IsActive = true,
                    Roles = new List<string>()
                }).ToList();

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"Found {userDTOs.Count} active users without role",
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

        public async Task<BaseResponseDTO> GetRoleStatisticsAsync()
        {
            try
            {
                var roles = await _userRoleRepository.GetAllRolesAsync();
                var stats = new List<object>();

                foreach (var role in roles)
                {
                    var count = await _userRoleRepository.CountUsersByRoleAsync(role.RoleId);
                    stats.Add(new
                    {
                        RoleId = role.RoleId,
                        RoleName = role.RoleName,
                        UserCount = count
                    });
                }

                var usersWithoutRole = await _userRoleRepository.CountUsersWithoutRoleAsync();

                var result = new
                {
                    RoleStatistics = stats,
                    UsersWithoutRole = usersWithoutRole
                };

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = "Statistics retrieved successfully",
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