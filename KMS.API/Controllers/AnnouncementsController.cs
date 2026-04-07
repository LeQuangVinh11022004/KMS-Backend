using KMS.Service.DTOs.Announcement;
using KMS.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnnouncementsController : ControllerBase
    {
        private readonly IAnnouncementService _service;

        public AnnouncementsController(IAnnouncementService service)
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
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AnnouncementDTO dto)
        {
            await _service.CreateAsync(dto, GetUserId());
            return Ok("Created successfully");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, AnnouncementDTO dto)
        {
            await _service.UpdateAsync(id, dto, GetUserId());
            return Ok("Updated successfully");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return Ok("Deleted successfully");
        }
        [HttpGet("by-class/{classId}")]
        public async Task<IActionResult> GetByClass(int classId)
        {
            var result = await _service.GetByClassIdAsync(classId);
            return Ok(result);
        }
    }
}
