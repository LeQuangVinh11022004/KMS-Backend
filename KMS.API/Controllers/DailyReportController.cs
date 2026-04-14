using KMS.Service.DTOs.DailyReport;
using KMS.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KMS.API.Controllers
{
    [Route("api/daily-reports")]
    [ApiController]
    [Authorize]
    public class DailyReportController : ControllerBase
    {
        private readonly IDailyReportService _service;

        public DailyReportController(IDailyReportService service)
        {
            _service = service;
        }
        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : 0;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Create([FromBody] CreateDailyReportDTO dto)
        {
            await _service.CreateAsync(dto, GetUserId());

            return Ok(new
            {
                message = "Daily report created successfully"
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDailyReportDTO dto)
        {
            await _service.UpdateAsync(id, dto);

            return Ok(new
            {
                message = "Daily report updated successfully"
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);

            return Ok(new
            {
                message = "Daily report deleted successfully"
            });
        }
    }
}
