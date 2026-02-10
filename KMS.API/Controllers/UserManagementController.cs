// UserManagementController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KMS.Service.Interfaces;
using System.Security.Claims;

namespace KMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")] // Chỉ Admin mới quản lý users
    public class UserManagementController : ControllerBase
    {
        private readonly IUserManagementService _userManagementService;
        private readonly ILogger<UserManagementController> _logger;

        public UserManagementController(
            IUserManagementService userManagementService,
            ILogger<UserManagementController> logger)
        {
            _userManagementService = userManagementService;
            _logger = logger;
        }

        /// <summary>
        /// Approve pending user (activate account)
        /// </summary>
        [HttpPost("users/{userId}/approve")]
        public async Task<IActionResult> ApproveUser(int userId)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var result = await _userManagementService.ApproveUserAsync(userId, currentUserId);

            if (!result.Success)
                return BadRequest(result);

            _logger.LogInformation($"Admin (UserId: {currentUserId}) approved UserId {userId}");

            return Ok(result);
        }

        /// <summary>
        /// Reject pending user
        /// </summary>
        [HttpPost("users/{userId}/reject")]
        public async Task<IActionResult> RejectUser(int userId)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var result = await _userManagementService.RejectUserAsync(userId, currentUserId);

            if (!result.Success)
                return BadRequest(result);

            _logger.LogInformation($"Admin (UserId: {currentUserId}) rejected UserId {userId}");

            return Ok(result);
        }

        /// <summary>
        /// Deactivate active user
        /// </summary>
        [HttpPost("users/{userId}/deactivate")]
        public async Task<IActionResult> DeactivateUser(int userId)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            // Prevent deactivating yourself
            if (userId == currentUserId)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "You cannot deactivate your own account"
                });
            }

            var result = await _userManagementService.DeactivateUserAsync(userId, currentUserId);

            if (!result.Success)
                return BadRequest(result);

            _logger.LogInformation($"Admin (UserId: {currentUserId}) deactivated UserId {userId}");

            return Ok(result);
        }

        /// <summary>
        /// Get all pending users (waiting for approval)
        /// </summary>
        [HttpGet("users/pending")]
        public async Task<IActionResult> GetPendingUsers()
        {
            var result = await _userManagementService.GetPendingUsersAsync();

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Get all active users
        /// </summary>
        [HttpGet("users/active")]
        public async Task<IActionResult> GetActiveUsers()
        {
            var result = await _userManagementService.GetActiveUsersAsync();

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Get all inactive users
        /// </summary>
        [HttpGet("users/inactive")]
        public async Task<IActionResult> GetInactiveUsers()
        {
            var result = await _userManagementService.GetInactiveUsersAsync();

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Get user status statistics
        /// </summary>
        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            var result = await _userManagementService.GetUserStatusStatisticsAsync();

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}