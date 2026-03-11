using KMS.Service.DTOs.AuditLog;
using KMS.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AuditLogsController : ControllerBase
    {
        private readonly IAuditLogService _auditLogService;

        public AuditLogsController(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        /// <summary>Get logs</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _auditLogService.GetAllAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Filter log by userId, action, tableName, fromDate, toDate
        /// </summary>
        [HttpGet("filter")]
        public async Task<IActionResult> GetFiltered([FromQuery] AuditLogFilterDTO filter)
        {
            var result = await _auditLogService.GetFilteredAsync(filter);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
