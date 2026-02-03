using KMS.Repository.Interfaces;
using KMS.Service.DTOs;
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

        public async Task<BaseResponseDTO> GetUserRolesAsync(int userId)
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

                var userRoles = await _userRoleRepository.GetUserRolesAsync(userId);
                var roleDTOs = userRoles.Select(ur => new RoleDTO
                {
                    RoleId = ur.Role.RoleId,
                    RoleName = ur.Role.RoleName,
                    Description = ur.Role.Description
                }).ToList();

                var userRoleDTO = new UserRoleDTO
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    FullName = user.FullName,
                    Roles = roleDTOs
                };

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = "User roles retrieved successfully",
                    Data = userRoleDTO
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
                        Message = "Failed to assign role. User or role not found, or user already has this role."
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
                        Message = "Failed to assign role. User not found or already has this role."
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

        public async Task<BaseResponseDTO> RemoveRoleFromUserAsync(int userId, int roleId)
        {
            try
            {
                var success = await _userRoleRepository.RemoveRoleFromUserAsync(userId, roleId);
                if (!success)
                {
                    return new BaseResponseDTO
                    {
                        Success = false,
                        Message = "Failed to remove role. User-role relationship not found."
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

        public async Task<BaseResponseDTO> GetAllUsersWithRolesAsync()
        {
            try
            {
                var allRoles = await _userRoleRepository.GetAllRolesAsync();
                var result = new List<UserRoleDTO>();

                foreach (var role in allRoles)
                {
                    var users = await _userRoleRepository.GetUsersByRoleAsync(role.RoleId);
                    foreach (var user in users)
                    {
                        var existingUser = result.FirstOrDefault(r => r.UserId == user.UserId);
                        if (existingUser == null)
                        {
                            var userRoles = await _userRoleRepository.GetUserRolesAsync(user.UserId);
                            result.Add(new UserRoleDTO
                            {
                                UserId = user.UserId,
                                Username = user.Username,
                                FullName = user.FullName,
                                Roles = userRoles.Select(ur => new RoleDTO
                                {
                                    RoleId = ur.Role.RoleId,
                                    RoleName = ur.Role.RoleName,
                                    Description = ur.Role.Description
                                }).ToList()
                            });
                        }
                    }
                }

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = "All users with roles retrieved successfully",
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