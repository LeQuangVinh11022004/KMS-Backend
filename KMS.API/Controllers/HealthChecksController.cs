using KMS.Service.DTOs.HealthCheck;
using KMS.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HealthChecksController : ControllerBase
    {
        private readonly IHealthCheckService _service;

        public HealthChecksController(IHealthCheckService service)
        {
            _service = service;
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetByStudent(int studentId)
            => Ok(await _service.GetByStudentIdAsync(studentId));

        [HttpPost]
        [Authorize(Roles = "Admin,Staff,Teacher")]
        public async Task<IActionResult> Create(HealthCheckDTO dto)
        {
            await _service.CreateAsync(dto, GetUserId());
            return Ok("Created successfully");
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Staff,Teacher")]
        public async Task<IActionResult> Update(int id, HealthCheckDTO dto)
        {
            await _service.UpdateAsync(id, dto, GetUserId());
            return Ok("Updated successfully");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Staff,Teacher")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return Ok("Deleted successfully");
        }
    }
}
