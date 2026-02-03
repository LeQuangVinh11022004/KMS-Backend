using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KMS.Service.DTOs;
using KMS.Service.Interfaces;
using System.Security.Claims;

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

        [HttpGet("roles")]
        public async Task<IActionResult> GetAllRoles()
        {
            var result = await _userRoleService.GetAllRolesAsync();

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("roles/{roleId}")]
        public async Task<IActionResult> GetRoleById(int roleId)
        {
            var result = await _userRoleService.GetRoleByIdAsync(roleId);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpGet("users/{userId}/roles")]
        public async Task<IActionResult> GetUserRoles(int userId)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var isAdmin = User.IsInRole("Admin");

            // Check permission: Admin or own profile
            if (!isAdmin && currentUserId != userId)
            {
                return Forbid();
            }

            var result = await _userRoleService.GetUserRolesAsync(userId);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpGet("users/all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsersWithRoles()
        {
            var result = await _userRoleService.GetAllUsersWithRolesAsync();

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("users/{userId}/roles")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignRole(int userId, [FromBody] AssignRoleRequestDTO request)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var result = await _userRoleService.AssignRoleToUserAsync(userId, request, currentUserId);

            if (!result.Success)
                return BadRequest(result);

            _logger.LogInformation($"Admin (UserId: {currentUserId}) assigned RoleId {request.RoleId} to UserId {userId}");

            return Ok(result);
        }

        [HttpPost("users/{userId}/roles/by-name")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignRoleByName(int userId, [FromBody] AssignRoleByNameRequestDTO request)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var result = await _userRoleService.AssignRoleByNameAsync(userId, request, currentUserId);

            if (!result.Success)
                return BadRequest(result);

            _logger.LogInformation($"Admin (UserId: {currentUserId}) assigned role '{request.RoleName}' to UserId {userId}");

            return Ok(result);
        }

        // ============================================================
        // REMOVE ROLE
        // ============================================================

        [HttpDelete("users/{userId}/roles/{roleId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveRole(int userId, int roleId)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            if (userId == currentUserId && roleId == 1)
            {
                return BadRequest(new BaseResponseDTO
                {
                    Success = false,
                    Message = "You cannot remove Admin role from yourself"
                });
            }

            var result = await _userRoleService.RemoveRoleFromUserAsync(userId, roleId);

            if (!result.Success)
                return BadRequest(result);

            _logger.LogInformation($"Admin (UserId: {currentUserId}) removed RoleId {roleId} from UserId {userId}");

            return Ok(result);
        }

        // ============================================================
        // GET USERS BY ROLE
        // ============================================================

        [HttpGet("roles/{roleId}/users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsersByRole(int roleId)
        {
            var result = await _userRoleService.GetUsersByRoleAsync(roleId);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("roles/by-name/{roleName}/users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsersByRoleName(string roleName)
        {
            var result = await _userRoleService.GetUsersByRoleNameAsync(roleName);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}