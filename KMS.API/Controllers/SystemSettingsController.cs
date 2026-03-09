using KMS.Service.DTOs.SystemSetting;
using KMS.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Principal")]
    public class SystemSettingsController : ControllerBase
    {
        private readonly ISystemSettingService _settingService;

        public SystemSettingsController(ISystemSettingService settingService)
        {
            _settingService = settingService;
        }

        /// <summary>Lấy tất cả cấu hình hệ thống</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _settingService.GetAllAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>Lấy cấu hình theo key (ví dụ: SchoolName)</summary>
        [HttpGet("{key}")]
        public async Task<IActionResult> GetByKey(string key)
        {
            var result = await _settingService.GetByKeyAsync(key);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Cập nhật 1 cấu hình theo key
        /// Body: { "settingValue": "Happy Kids School" }
        /// </summary>
        [HttpPut("{key}")]
        public async Task<IActionResult> Update(string key, [FromBody] UpdateSettingDTO dto)
        {
            var updatedBy = GetCurrentUserId();
            var result = await _settingService.UpdateAsync(key, dto, updatedBy);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Cập nhật nhiều cấu hình cùng lúc
        /// Body: { "settings": { "SchoolName": "ABC", "SchoolPhone": "0901234567" } }
        /// </summary>
        [HttpPut("batch")]
        public async Task<IActionResult> UpdateMultiple([FromBody] UpdateMultipleSettingsDTO dto)
        {
            var updatedBy = GetCurrentUserId();
            var result = await _settingService.UpdateMultipleAsync(dto, updatedBy);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        private int GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out int id) ? id : 0;
        }
    }
}
