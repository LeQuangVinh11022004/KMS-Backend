using KMS.Service.DTOs.Menu;
using KMS.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MenusController : ControllerBase
    {
        private readonly IMenuService _service;

        public MenusController(IMenuService service)
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
            var data = await _service.GetByIdAsync(id);
            if (data == null) return NotFound();
            return Ok(data);
        }

        [HttpGet("by-class/{classId}")]
        public async Task<IActionResult> GetByClass(int classId)
            => Ok(await _service.GetByClassIdAsync(classId));

        [HttpPost]
        public async Task<IActionResult> Create(MenuCreateDTO dto)
        {
            await _service.CreateAsync(dto, GetUserId());
            return Ok("Menu created successfully");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, MenuUpdateDTO dto)
        {
            var ok = await _service.UpdateAsync(id, dto, GetUserId());
            if (!ok) return NotFound();
            return Ok("Menu updated successfully");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _service.DeleteAsync(id);
            if (!ok) return NotFound();
            return Ok("Menu deleted");
        }
    }
}
