using KMS.Repository.Interfaces;
using KMS.Service.DTOs;
using KMS.Service.DTOs.Attendance;
using KMS.Service.DTOs.Role;
using KMS.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;
        private readonly ITeacherRepository _teacherRepository;

        public AttendanceController(
            IAttendanceService attendanceService,
            ITeacherRepository teacherRepository)
        {
            _attendanceService = attendanceService;
            _teacherRepository = teacherRepository;
        }

        /// <summary>
        /// List class by day (EX: date=2026-03-13)
        /// </summary>
        [HttpGet("class/{classId}")]
        [Authorize(Roles = "Teacher,Admin,Staff")]
        public async Task<IActionResult> GetClassSheet(int classId, [FromQuery] DateTime? date)
        {
            var targetDate = date ?? DateTime.Today;
            var result = await _attendanceService.GetClassAttendanceSheetAsync(classId, targetDate);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Teacher save attendance for 1 student (can be use for many student)
        /// </summary>
        [HttpPost("bulk")]
        [Authorize(Roles = "Teacher,Admin,Staff")]
        public async Task<IActionResult> SubmitBulk([FromBody] BulkAttendanceDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var teacherId = await GetCurrentTeacherIdAsync();
            if (teacherId == 0)
                return Unauthorized(new BaseResponseDTO
                {
                    Success = false,
                    Message = "Không tìm thấy thông tin giáo viên"
                });

            var result = await _attendanceService.SubmitBulkAttendanceAsync(dto, teacherId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Teacher update attendance for 1 student 
        /// </summary>
        [HttpPut("{attendanceId}")]
        [Authorize(Roles = "Teacher,Admin,Staff")]
        public async Task<IActionResult> Update(int attendanceId, [FromBody] UpdateAttendanceDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var teacherId = await GetCurrentTeacherIdAsync();
            if (teacherId == 0)
                return Unauthorized(new BaseResponseDTO
                {
                    Success = false,
                    Message = "Không tìm thấy thông tin giáo viên"
                });

            var result = await _attendanceService.UpdateAttendanceAsync(attendanceId, dto, teacherId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Teacher check out 1 attendance record (set checkout time = now)
        /// </summary>
        [HttpPatch("{attendanceId}/checkout")]
        [Authorize(Roles = "Teacher,Admin,Staff")]
        public async Task<IActionResult> CheckOut(int attendanceId, [FromBody] CheckOutDTO dto)
        {
            var teacherId = await GetCurrentTeacherIdAsync();
            if (teacherId == 0)
                return Unauthorized(new BaseResponseDTO
                {
                    Success = false,
                    Message = "Không tìm thấy thông tin giáo viên"
                });

            var result = await _attendanceService.CheckOutAsync(attendanceId, dto, teacherId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Check attendance history of a student (EX: fromDate=2026-03-01 to Date=2026-03-31)
        /// </summary>
        [HttpGet("student/{studentId}")]
        [Authorize(Roles = "Teacher,Admin,Parent,Staff")]
        public async Task<IActionResult> GetStudentHistory(
            int studentId,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            var result = await _attendanceService.GetStudentHistoryAsync(studentId, fromDate, toDate);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // HELPER

        private int GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out int id) ? id : 0;
        }

        private async Task<int> GetCurrentTeacherIdAsync()
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return 0;
            var teacher = await _teacherRepository.GetByUserIdAsync(userId);
            return teacher?.TeacherId ?? 0;
        }
    }
}