using KMS.Service.DTOs;
using KMS.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        /// <summary>
        /// Get all students
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllStudents()
        {
            var result = await _studentService.GetAllStudentsAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get student by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudentById(int id)
        {
            var result = await _studentService.GetStudentByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Create new student
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateStudent([FromBody] CreateStudentDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _studentService.CreateStudentAsync(dto);
            return result.Success ? CreatedAtAction(nameof(GetStudentById), new { id = ((StudentDTO)result.Data).StudentId }, result) : BadRequest(result);
        }

        /// <summary>
        /// Update student
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] UpdateStudentDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _studentService.UpdateStudentAsync(id, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Delete student
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var result = await _studentService.DeleteStudentAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Search students by keyword
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> SearchStudents([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest(new BaseResponseDTO { Success = false, Message = "Keyword is required" });

            var result = await _studentService.SearchStudentsAsync(keyword);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get all active students
        /// </summary>
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveStudents()
        {
            var result = await _studentService.GetActiveStudentsAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get students by parent ID
        /// </summary>
        [HttpGet("parent/{parentId}")]
        public async Task<IActionResult> GetStudentsByParentId(int parentId)
        {
            var result = await _studentService.GetStudentsByParentIdAsync(parentId);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}