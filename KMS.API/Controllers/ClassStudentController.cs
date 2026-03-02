using KMS.Service.DTOs;
using KMS.Service.DTOs.ClassStudent;
using KMS.Service.DTOs.ClassTeacher;
using KMS.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KMS.API.Controllers
{
    // ============================================================
    // CLASS STUDENT CONTROLLER
    // ============================================================

    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Principal")]
    public class ClassStudentController : ControllerBase
    {
        private readonly IClassStudentService _classStudentService;

        public ClassStudentController(IClassStudentService classStudentService)
        {
            _classStudentService = classStudentService;
        }

        /// <summary>Lấy danh sách học sinh trong lớp</summary>
        [HttpGet("class/{classId}")]
        public async Task<IActionResult> GetStudentsByClass(int classId)
        {
            var result = await _classStudentService.GetStudentsByClassAsync(classId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>Lấy danh sách lớp của học sinh</summary>
        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetClassesByStudent(int studentId)
        {
            var result = await _classStudentService.GetClassesByStudentAsync(studentId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>Xếp 1 học sinh vào lớp</summary>
        [HttpPost("class/{classId}/enroll")]
        public async Task<IActionResult> EnrollStudent(int classId, [FromBody] EnrollStudentDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var enrolledBy = GetCurrentUserId();
            var result = await _classStudentService.EnrollStudentAsync(classId, dto, enrolledBy);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Xếp nhiều học sinh vào lớp cùng lúc
        /// Body: { "studentIds": [1, 2, 3] }
        /// </summary>
        [HttpPost("class/{classId}/enroll-multiple")]
        public async Task<IActionResult> EnrollMultiple(int classId, [FromBody] EnrollMultipleStudentsDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var enrolledBy = GetCurrentUserId();
            var result = await _classStudentService.EnrollMultipleStudentsAsync(classId, dto, enrolledBy);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>Xóa học sinh khỏi lớp</summary>
        [HttpDelete("class/{classId}/student/{studentId}")]
        public async Task<IActionResult> RemoveStudent(int classId, int studentId)
        {
            var result = await _classStudentService.RemoveStudentFromClassAsync(classId, studentId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Chuyển học sinh từ lớp này sang lớp khác
        /// Query: fromClassId, toClassId
        /// </summary>
        [HttpPost("student/{studentId}/transfer")]
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