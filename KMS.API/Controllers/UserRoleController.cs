using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KMS.Service.Interfaces;
using System.Security.Claims;
using KMS.Service.DTOs.Role;

namespace KMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserRoleController : ControllerBase
    {
        private readonly IUserRoleService _userRoleService;
        private readonly ILogger<UserRoleController> _logger;

        public UserRoleController(IUserRoleService userRoleService, ILogger<UserRoleController> logger)
        {
            _userRoleService = userRoleService;
            _logger = logger;
        }

        /// <summary>
        /// Get all roles
        /// </summary>
        [HttpGet("roles")]
        public async Task<IActionResult> GetAllRoles()
        {
            var result = await _userRoleService.GetAllRolesAsync();
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
        /// <summary>
        /// Get role by roleID
        /// </summary>
        [HttpGet("roles/{roleId}")]
        public async Task<IActionResult> GetRoleById(int roleId)
        {
            var result = await _userRoleService.GetRoleByIdAsync(roleId);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        /// <summary>
        /// Get role of a specific user
        /// </summary>
        [HttpGet("users/{userId}/role")]
        public async Task<IActionResult> GetUserRole(int userId)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && currentUserId != userId)
            {
                return Forbid();
            }

            var result = await _userRoleService.GetUserRoleAsync(userId);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        /// <summary>
        /// Assign role(roleID) by userID
        /// </summary>
        [HttpPost("users/{userId}/roles")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignRole(int userId, [FromBody] AssignRoleRequestDTO request)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var result = await _userRoleService.AssignRoleToUserAsync(userId, request, currentUserId);

            if (!result.Success) return BadRequest(result);

            _logger.LogInformation($"Admin (UserId: {currentUserId}) assigned RoleId {request.RoleId} to UserId {userId}");
            return Ok(result);
        }
        /// <summary>
        /// Assign role(roleName) by userID
        /// </summary>
        [HttpPost("users/{userId}/roles/by-name")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignRoleByName(int userId, [FromBody] AssignRoleByNameRequestDTO request)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var result = await _userRoleService.AssignRoleByNameAsync(userId, request, currentUserId);

            if (!result.Success) return BadRequest(result);

            _logger.LogInformation($"Admin (UserId: {currentUserId}) assigned role '{request.RoleName}' to UserId {userId}");
            return Ok(result);
        }

        /// <summary>
        /// Update user role
        /// </summary>
        [HttpPut("users/{userId}/role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUserRole(int userId, [FromBody] AssignRoleRequestDTO request)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var result = await _userRoleService.UpdateUserRoleAsync(userId, request, currentUserId);

            if (!result.Success) return BadRequest(result);

            _logger.LogInformation($"Admin (UserId: {currentUserId}) updated role for UserId {userId} to RoleId {request.RoleId}");
            return Ok(result);
        }

        /// <summary>
        /// Remove user role
        /// </summary>
        [HttpDelete("users/{userId}/role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveRole(int userId)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            // Prevent removing role from yourself
            if (userId == currentUserId)
            {
                return BadRequest(new BaseResponseDTO
                {
                    Success = false,
                    Message = "You cannot remove role from yourself"
                });
            }

            var result = await _userRoleService.RemoveRoleFromUserAsync(userId);

            if (!result.Success) return BadRequest(result);

            _logger.LogInformation($"Admin (UserId: {currentUserId}) removed role from UserId {userId}");
            return Ok(result);
        }

        /// <summary>
        /// List user by roleID
        /// </summary>
        [HttpGet("roles/{roleId}/users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsersByRole(int roleId)
        {
            var result = await _userRoleService.GetUsersByRoleAsync(roleId);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
        /// <summary>
        /// List user by roleName
        /// </summary>
        [HttpGet("roles/by-name/{roleName}/users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsersByRoleName(string roleName)
        {
            var result = await _userRoleService.GetUsersByRoleNameAsync(roleName);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        /// <summary>
        /// Get all users without any role
        /// </summary>
        [HttpGet("users/without-role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsersWithoutRole()
        {
            var result = await _userRoleService.GetUsersWithoutRoleAsync();
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        /// <summary>
        /// Get active users without role
        /// </summary>
        [HttpGet("users/active-without-role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetActiveUsersWithoutRole()
        {
            var result = await _userRoleService.GetActiveUsersWithoutRoleAsync();
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        /// <summary>
        /// Get role statistics (count users per role)
        /// </summary>
        [HttpGet("statistics")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetStatistics()
        {
            var result = await _userRoleService.GetRoleStatisticsAsync();
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
    }
}