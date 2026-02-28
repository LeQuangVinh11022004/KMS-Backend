using KMS.Service.DTOs;
using KMS.Service.DTOs.Role;
using KMS.Service.Interfaces;
using KMS.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParentRegistrationController : ControllerBase
    {
        private readonly IParentRegistrationService _registrationService;

        public ParentRegistrationController(IParentRegistrationService registrationService)
        {
            _registrationService = registrationService;
        }

        // ============================================================
        // PUBLIC - Phụ huynh tự submit (không cần đăng nhập)
        // ============================================================

        /// <summary>
        /// Phụ huynh nộp đơn đăng ký tài khoản
        /// </summary>
        [HttpPost("submit")]
        [AllowAnonymous]
        public async Task<IActionResult> Submit([FromBody] CreateParentRegistrationDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _registrationService.SubmitRegistrationAsync(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // ============================================================
        // ADMIN - Xem danh sách
        // ============================================================

        /// <summary>
        /// Lấy tất cả đơn đăng ký (Admin)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin,Principal")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _registrationService.GetAllRegistrationsAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Lấy đơn đăng ký đang chờ duyệt (Admin)
        /// </summary>
        [HttpGet("pending")]
        [Authorize(Roles = "Admin,Principal")]
        public async Task<IActionResult> GetPending()
        {
            var result = await _registrationService.GetPendingRegistrationsAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Lấy chi tiết 1 đơn đăng ký (Admin)
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Principal")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _registrationService.GetRegistrationByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        // ============================================================
        // ADMIN - Duyệt / Từ chối
        // ============================================================

        /// <summary>
        /// Duyệt đơn → Tự động tạo User + Parent + gán Role "Parent"
        /// Response trả về username và mật khẩu tạm để Admin gửi cho phụ huynh
        /// </summary>
        [HttpPost("{id}/approve")]
        [Authorize(Roles = "Admin,Principal")]
        public async Task<IActionResult> Approve(int id, [FromBody] ReviewRegistrationDTO dto)
        {
            var reviewedBy = GetCurrentUserId();
            if (reviewedBy == 0)
                return Unauthorized(new BaseResponseDTO { Success = false, Message = "Invalid token" });

            var result = await _registrationService.ApproveRegistrationAsync(id, reviewedBy, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Từ chối đơn đăng ký (Admin)
        /// </summary>
        [HttpPost("{id}/reject")]
        [Authorize(Roles = "Admin,Principal")]
        public async Task<IActionResult> Reject(int id, [FromBody] ReviewRegistrationDTO dto)
        {
            var reviewedBy = GetCurrentUserId();
            if (reviewedBy == 0)
                return Unauthorized(new BaseResponseDTO { Success = false, Message = "Invalid token" });

            var result = await _registrationService.RejectRegistrationAsync(id, reviewedBy, dto);
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