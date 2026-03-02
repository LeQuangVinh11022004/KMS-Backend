using KMS.Service.DTOs;
using KMS.Service.DTOs.StudentParent;
using KMS.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Principal")]
    public class StudentParentController : ControllerBase
    {
        private readonly IStudentParentService _studentParentService;

        public StudentParentController(IStudentParentService studentParentService)
        {
            _studentParentService = studentParentService;
        }

        /// <summary>Lấy danh sách phụ huynh của học sinh</summary>
        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetParentsByStudent(int studentId)
        {
            var result = await _studentParentService.GetParentsByStudentAsync(studentId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>Lấy danh sách học sinh của phụ huynh</summary>
        [HttpGet("parent/{parentId}")]
        public async Task<IActionResult> GetStudentsByParent(int parentId)
        {
            var result = await _studentParentService.GetStudentsByParentAsync(parentId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Gán phụ huynh cho học sinh
        /// Relationship: Father | Mother | Guardian | Other
        /// Người đầu tiên được gán tự động thành PrimaryContact
        /// </summary>
        [HttpPost("student/{studentId}/link")]
        public async Task<IActionResult> Link(int studentId, [FromBody] LinkStudentParentDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _studentParentService.LinkAsync(studentId, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Cập nhật quan hệ hoặc đổi PrimaryContact
        /// </summary>
        [HttpPatch("student/{studentId}/parent/{parentId}")]
        public async Task<IActionResult> UpdateLink(
            int studentId, int parentId, [FromBody] UpdateStudentParentDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _studentParentService.UpdateLinkAsync(studentId, parentId, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Hủy liên kết phụ huynh - học sinh
        /// Nếu xóa PrimaryContact thì tự động set người tiếp theo làm Primary
        /// </summary>
        [HttpDelete("student/{studentId}/parent/{parentId}")]
        public async Task<IActionResult> Unlink(int studentId, int parentId)
        {
            var result = await _studentParentService.UnlinkAsync(studentId, parentId);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}