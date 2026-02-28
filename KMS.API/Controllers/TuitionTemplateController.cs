using KMS.Service.DTOs;
using KMS.Service.DTOs.TuitionTemplate;
using KMS.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KMS.API.Controllers
{
    // ============================================================
    // TUITION TEMPLATE CONTROLLER
    // ============================================================

    [ApiController]
    [Route("api/[controller]")]
    public class TuitionTemplateController : ControllerBase
    {
        private readonly ITuitionTemplateService _templateService;

        public TuitionTemplateController(ITuitionTemplateService templateService)
        {
            _templateService = templateService;
        }

        /// <summary>Get all tuition templates</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _templateService.GetAllTemplatesAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>Get active templates only</summary>
        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var result = await _templateService.GetActiveTemplatesAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>Get template by ID (includes items)</summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _templateService.GetTemplateByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>Create new tuition template with items</summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTuitionTemplateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _templateService.CreateTemplateAsync(dto);
            return result.Success
                ? CreatedAtAction(nameof(GetById), new { id = ((TuitionTemplateDTO)result.Data).TemplateId }, result)
                : BadRequest(result);
        }

        /// <summary>Update template (pass Items to replace all items)</summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTuitionTemplateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _templateService.UpdateTemplateAsync(id, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>Delete template (blocked if used by invoices)</summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _templateService.DeleteTemplateAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>Activate / Deactivate template</summary>
        [HttpPatch("{id}/toggle-status")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var result = await _templateService.ToggleTemplateStatusAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }
    }
}