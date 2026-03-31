using KMS.Service.DTOs.ClassTeacher;
using KMS.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KMS.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Teacher")]
    public class ClassTeacherController : ControllerBase
    {
        private readonly IClassTeacherService _classTeacherService;

        public ClassTeacherController(IClassTeacherService classTeacherService)
        {
            _classTeacherService = classTeacherService;
        }

        /// <summary>List class teacher by classID</summary>
        [HttpGet("class/{classId}")]
        public async Task<IActionResult> GetTeachersByClass(int classId)
        {
            var result = await _classTeacherService.GetTeachersByClassAsync(classId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>List class teacher by teacherID</summary>
        [HttpGet("teacher/{teacherId}")]
        public async Task<IActionResult> GetClassesByTeacher(int teacherId)
        {
            var result = await _classTeacherService.GetClassesByTeacherAsync(teacherId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Assign teacher to class with specific role(each class just have 1 "Main" teacher)
        /// </summary>
        [HttpPost("class/{classId}/assign")]
        public async Task<IActionResult> AssignTeacher(int classId, [FromBody] AssignTeacherDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var assignedBy = GetCurrentUserId();
            var result = await _classTeacherService.AssignTeacherAsync(classId, dto, assignedBy);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Update teacher role in class (Main/Support)
        /// </summary>
        [HttpPatch("class/{classId}/teacher/{teacherId}/role")]
        public async Task<IActionResult> UpdateRole(
            int classId, int teacherId, [FromBody] UpdateTeacherRoleDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _classTeacherService.UpdateTeacherRoleAsync(classId, teacherId, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Delete teacher from class
        /// </summary>
        [HttpDelete("class/{classId}/teacher/{teacherId}")]
        public async Task<IActionResult> RemoveTeacher(int classId, int teacherId)
        {
            var result = await _classTeacherService.RemoveTeacherFromClassAsync(classId, teacherId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        private int GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out int id) ? id : 0;
        }
    }
}
