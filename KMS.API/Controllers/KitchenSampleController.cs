using KMS.Service.DTOs.KitchenSample;
using KMS.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KitchenSampleController : ControllerBase
    {
        private readonly IKitchenSampleService _service;

        public KitchenSampleController(IKitchenSampleService service)
        {
            _service = service;
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.GetAllAsync();
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _service.GetByIdAsync(id);

            if (data == null)
                return NotFound("Kitchen sample not found");

            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create(KitchenSampleDTO dto)
        {
            var userId = GetUserId();

            await _service.CreateAsync(dto, userId);

            return Ok(new
            {
                message = "Kitchen sample created successfully"
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, KitchenSampleDTO dto)
        {
            await _service.UpdateAsync(id, dto);

            return Ok(new
            {
                message = "Kitchen sample updated successfully"
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);

            return Ok(new
            {
                message = "Kitchen sample deleted successfully"
            });
        }
    }
}