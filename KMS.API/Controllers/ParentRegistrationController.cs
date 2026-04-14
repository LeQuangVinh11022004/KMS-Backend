using KMS.Service.DTOs;
using KMS.Service.DTOs.Role;
using KMS.Service.Interfaces;
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

        /// <summary>
        /// Parent submit registeration request
        /// </summary>
        [HttpPost("submit")]
        [AllowAnonymous]
        public async Task<IActionResult> Submit([FromBody] CreateParentRegistrationDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _registrationService.SubmitRegistrationAsync(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get all registration requests (Admin)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _registrationService.GetAllRegistrationsAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get pending registration requests (Admin)
        /// </summary>
        [HttpGet("pending")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPending()
        {
            var result = await _registrationService.GetPendingRegistrationsAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get registration request by ID (Admin)
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _registrationService.GetRegistrationByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Approve registration request (Admin)
        /// </summary>
        [HttpPost("{id}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(int id, [FromBody] ReviewRegistrationDTO dto)
        {
            var reviewedBy = GetCurrentUserId();
            if (reviewedBy == 0)
                return Unauthorized(new BaseResponseDTO { Success = false, Message = "Invalid token" });

            var result = await _registrationService.ApproveRegistrationAsync(id, reviewedBy, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Reject registration request (Admin)
        /// </summary>
        [HttpPost("{id}/reject")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reject(int id, [FromBody] ReviewRegistrationDTO dto)
        {
            var reviewedBy = GetCurrentUserId();
            if (reviewedBy == 0)
                return Unauthorized(new BaseResponseDTO { Success = false, Message = "Invalid token" });

            var result = await _registrationService.RejectRegistrationAsync(id, reviewedBy, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        private int GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out int id) ? id : 0;
        }
    }
}