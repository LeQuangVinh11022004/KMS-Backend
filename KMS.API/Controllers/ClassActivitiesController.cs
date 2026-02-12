using KMS.Service.DTOs.ClassActivity;
using KMS.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassActivitiesController : ControllerBase
    {
        private readonly IClassActivityService _service;

        public ClassActivitiesController(IClassActivityService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("class/{classId}")]
        public async Task<IActionResult> GetByClass(int classId)
        {
            return Ok(await _service.GetByClassIdAsync(classId));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ClassActivityDTO dto)
        {
            await _service.CreateAsync(dto, 1); // temp teacherId
            return Ok("Created successfully");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ClassActivityDTO dto)
        {
            await _service.UpdateAsync(id, dto, 1);
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
