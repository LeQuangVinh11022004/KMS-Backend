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

        /// <summary>
        /// Get parent by studentID
        /// </summary>
        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetParentsByStudent(int studentId)
        {
            var result = await _studentParentService.GetParentsByStudentAsync(studentId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Get student by parentID
        /// </summary>
        [HttpGet("parent/{parentId}")]
        public async Task<IActionResult> GetStudentsByParent(int parentId)
        {
            var result = await _studentParentService.GetStudentsByParentAsync(parentId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Assign relationship between student and parent
        /// </summary>
        [HttpPost("student/{studentId}/link")]
        public async Task<IActionResult> Link(int studentId, [FromBody] LinkStudentParentDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _studentParentService.LinkAsync(studentId, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Update relationship between student and parent (relationship type, primary contact)
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
        /// Delete relationship between student and parent
        /// </summary>
        [HttpDelete("student/{studentId}/parent/{parentId}")]
        public async Task<IActionResult> Unlink(int studentId, int parentId)
        {
            var result = await _studentParentService.UnlinkAsync(studentId, parentId);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}