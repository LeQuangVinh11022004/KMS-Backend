using KMS.Service.DTOs;
using KMS.Service.DTOs.ResetPassword;
using KMS.Service.DTOs.Role;
using KMS.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PasswordController : ControllerBase
    {
        private readonly IPasswordService _passwordService;

        public PasswordController(IPasswordService passwordService)
        {
            _passwordService = passwordService;
        }

        // ============================================================
        // ADMIN
        // ============================================================

        /// <summary>
        /// Admin reset password for user 
        /// </summary>
        [HttpPost("admin-reset/{userId}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> AdminReset(int userId)
        {
            var adminId = GetCurrentUserId();
            if (adminId == 0)
                return Unauthorized(new BaseResponseDTO { Success = false, Message = "Invalid token" });

            if (adminId == userId)
                return BadRequest(new BaseResponseDTO
                {
                    Success = false,
                    Message = "Use /change to update your own password"
                });

            var result = await _passwordService.AdminResetPasswordAsync(userId, adminId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // ============================================================
        // USER (cần đăng nhập)
        // ============================================================

        /// <summary>
        /// User change password
        /// </summary>
        [HttpPost("change")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized(new BaseResponseDTO { Success = false, Message = "Invalid token" });

            var result = await _passwordService.ChangePasswordAsync(userId, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // ============================================================
        // FORGOT PASSWORD (không cần đăng nhập)
        // ============================================================

        /// <summary>
        /// User forgot password
        /// </summary>
        [HttpPost("forgot")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _passwordService.ForgotPasswordAsync(dto);
            return Ok(result);
        }

        /// <summary>
        /// Reset password with token
        /// </summary>
        [HttpPost("reset")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetWithToken([FromBody] ResetPasswordDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _passwordService.ResetPasswordWithTokenAsync(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // ============================================================
        // HELPER
        // ============================================================

        private int GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out int id) ? id : 0;
        }
    }
}