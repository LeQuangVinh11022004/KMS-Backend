using KMS.Service.DTOs;
using KMS.Service.DTOs.ClassStudent;
using KMS.Service.DTOs.ClassTeacher;
using KMS.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KMS.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ClassStudentController : ControllerBase
    {
        private readonly IClassStudentService _classStudentService;

        public ClassStudentController(IClassStudentService classStudentService)
        {
            _classStudentService = classStudentService;
        }

        /// <summary>Get list student by classID</summary>
        [HttpGet("class/{classId}")]
        public async Task<IActionResult> GetStudentsByClass(int classId)
        {
            var result = await _classStudentService.GetStudentsByClassAsync(classId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>Get list student by studentID</summary>
        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetClassesByStudent(int studentId)
        {
            var result = await _classStudentService.GetClassesByStudentAsync(studentId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>Assign student to class</summary>
        [HttpPost("class/{classId}/enroll")]
        [Authorize(Roles = "Admin,Teacher,Staff")]
        public async Task<IActionResult> EnrollStudent(int classId, [FromBody] EnrollStudentDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var enrolledBy = GetCurrentUserId();
            var result = await _classStudentService.EnrollStudentAsync(classId, dto, enrolledBy);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Assign mutilple students to class
        /// Body: { "studentIds": [1, 2, 3] }
        /// </summary>
        [HttpPost("class/{classId}/enroll-multiple")]
        [Authorize(Roles = "Admin,Teacher,Staff")]
        public async Task<IActionResult> EnrollMultiple(int classId, [FromBody] EnrollMultipleStudentsDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var enrolledBy = GetCurrentUserId();
            var result = await _classStudentService.EnrollMultipleStudentsAsync(classId, dto, enrolledBy);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>Delete student from class</summary>
        [HttpDelete("class/{classId}/student/{studentId}")]
        [Authorize(Roles = "Admin,Teacher,Staff")]
        public async Task<IActionResult> RemoveStudent(int classId, int studentId)
        {
            var result = await _classStudentService.RemoveStudentFromClassAsync(classId, studentId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Switch student to another class
        /// </summary>
        [HttpPost("student/{studentId}/transfer")]
        [Authorize(Roles = "Admin,Teacher,Staff")]
        public async Task<IActionResult> Transfer(
            int studentId,
            [FromQuery] int fromClassId,
            [FromQuery] int toClassId)
        {
            var enrolledBy = GetCurrentUserId();
            var result = await _classStudentService.TransferStudentAsync(
                studentId, fromClassId, toClassId, enrolledBy);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        private int GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out int id) ? id : 0;
        }
    }

    
}