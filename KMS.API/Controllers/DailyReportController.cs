using KMS.Service.DTOs.DailyReport;
using KMS.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KMS.API.Controllers
{
    [Route("api/daily-reports")]
    [ApiController]
    public class DailyReportController : ControllerBase
    {
        private readonly IDailyReportService _service;

        public DailyReportController(IDailyReportService service)
        {
            _service = service;
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
        public async Task<IActionResult> Create([FromBody] CreateDailyReportDTO dto)
        {
            await _service.CreateAsync(dto);

            return Ok(new
            {
                message = "Daily report created successfully"
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDailyReportDTO dto)
        {
            await _service.UpdateAsync(id, dto);

            return Ok(new
            {
                message = "Daily report updated successfully"
            });
        }

        [HttpDelete("{id}")]
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
