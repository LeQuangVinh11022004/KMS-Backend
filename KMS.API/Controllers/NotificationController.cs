using KMS.Service.DTOs.Notification;
using KMS.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _service;

        public NotificationController(INotificationService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> MyNotifications()
        {
            var userId = GetCurrentUserId();
            var result = await _service.GetUserNotificationsAsync(userId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateNotificationDTO dto)
        {
            var userId = GetCurrentUserId();

            var result = await _service.CreateAsync(dto, userId);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateNotificationDTO dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            return Ok(result);
        }

        [HttpPatch("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var result = await _service.MarkAsReadAsync(id);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            return Ok(result);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var result = await _service.GetUserNotificationsAsync(userId);
            return Ok(result);
        }

        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetByStudentId(int studentId)
        {
            var result = await _service.GetStudentNotificationsAsync(studentId);
            return Ok(result);
        }

        private int GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out int id) ? id : 0;
        }
    }
}
