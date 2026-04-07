using KMS.Service.DTOs.Class;
using KMS.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KMS.API.Controllers
{
    [ApiController]
    [Route("api/classes")]
    public class ClassesController : ControllerBase
    {
        private readonly IClassService _service;

        public ClassesController(IClassService service)
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
            => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ClassCreateRequestDTO request)
        {
            var result = await _service.CreateAsync(request, GetUserId());
            return CreatedAtAction(nameof(GetById), new { id = result.ClassId }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ClassUpdateRequestDTO request)
        {
            var success = await _service.UpdateAsync(id, request);
            return success ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            return success ? NoContent() : NotFound();
        }
    }
}
