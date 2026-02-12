using KMS.Service.DTOs.Campus;
using KMS.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampusesController : ControllerBase
    {
        private readonly ICampusService _service;

        public CampusesController(ICampusService service)
        {
            _service = service;
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
        public async Task<IActionResult> Create(CampusDTO dto)
        {
            await _service.CreateAsync(dto);
            return Ok("Created successfully");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CampusDTO dto)
        {
            await _service.UpdateAsync(id, dto);
            return Ok("Updated successfully");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return Ok("Deleted successfully");
        }
    }
}
