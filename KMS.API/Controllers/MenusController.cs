using KMS.Service.DTOs.Menu;
using KMS.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KMS.API.Controllers
{
    [ApiController]
    [Route("api/menus")]
    public class MenusController : ControllerBase
    {
        private readonly IMenuService _service;

        public MenusController(IMenuService service)
        {
            _service = service;
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
            await _service.CreateAsync(dto, createdBy: 1);
            return Ok("Menu created");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, MenuUpdateDTO dto)
        {
            var ok = await _service.UpdateAsync(id, dto, updatedBy: 1);
            if (!ok) return NotFound();
            return Ok("Menu updated");
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
