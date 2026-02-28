using KMS.Service.DTOs;
using KMS.Service.DTOs.Parent;
using KMS.Service.DTOs.Role;
using KMS.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParentController : ControllerBase
    {
        private readonly IParentService _parentService;

        public ParentController(IParentService parentService)
        {
            _parentService = parentService;
        }

        /// <summary>
        /// Get all parents
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllParents()
        {
            var result = await _parentService.GetAllParentsAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get parent by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetParentById(int id)
        {
            var result = await _parentService.GetParentByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Create new parent
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateParent([FromBody] CreateParentDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _parentService.CreateParentAsync(dto);
            return result.Success ? CreatedAtAction(nameof(GetParentById), new { id = ((ParentDTO)result.Data).ParentId }, result) : BadRequest(result);
        }

        /// <summary>
        /// Update parent
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateParent(int id, [FromBody] UpdateParentDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _parentService.UpdateParentAsync(id, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Delete parent
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteParent(int id)
        {
            var result = await _parentService.DeleteParentAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get parents by student ID
        /// </summary>
        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetParentsByStudentId(int studentId)
        {
            var result = await _parentService.GetParentsByStudentIdAsync(studentId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Search parents by keyword
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> SearchParents([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest(new BaseResponseDTO { Success = false, Message = "Keyword is required" });

            var result = await _parentService.SearchParentsAsync(keyword);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}