using KMS.Service.DTOs;
using KMS.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeacherController : ControllerBase
    {
        private readonly ITeacherService _teacherService;

        public TeacherController(ITeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        /// <summary>
        /// Get all teachers
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllTeachers()
        {
            var result = await _teacherService.GetAllTeachersAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get teacher by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTeacherById(int id)
        {
            var result = await _teacherService.GetTeacherByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Create new teacher
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateTeacher([FromBody] CreateTeacherDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _teacherService.CreateTeacherAsync(dto);
            return result.Success ? CreatedAtAction(nameof(GetTeacherById), new { id = ((TeacherDTO)result.Data).TeacherId }, result) : BadRequest(result);
        }

        /// <summary>
        /// Update teacher
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTeacher(int id, [FromBody] UpdateTeacherDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _teacherService.UpdateTeacherAsync(id, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Delete teacher
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeacher(int id)
        {
            var result = await _teacherService.DeleteTeacherAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Search teachers by keyword
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> SearchTeachers([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest(new BaseResponseDTO { Success = false, Message = "Keyword is required" });

            var result = await _teacherService.SearchTeachersAsync(keyword);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get all active teachers
        /// </summary>
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveTeachers()
        {
            var result = await _teacherService.GetActiveTeachersAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}