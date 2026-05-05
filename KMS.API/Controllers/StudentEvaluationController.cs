using KMS.Service.DTOs.Evaluation;
using KMS.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KMS.API.Controllers
{
    [ApiController]
    [Route("api/students/{studentId:int}/evaluations")]
    [Authorize]
    public class StudentEvaluationController : ControllerBase
    {
        private readonly IEvaluationService _service;

        public StudentEvaluationController(IEvaluationService service)
        {
            _service = service;
        }

        // GET /api/students/{studentId}/evaluations?periodType=monthly
        [HttpGet]
        public async Task<IActionResult> GetByStudent(
            int studentId,
            [FromQuery] string? periodType = null)
        {
            var result = await _service.GetByStudentIdAsync(studentId, periodType);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // GET /api/students/{studentId}/evaluations/{evaluationId}
        [HttpGet("{evaluationId:int}")]
        public async Task<IActionResult> GetById(int studentId, int evaluationId)
        {
            var result = await _service.GetByIdAsync(evaluationId);
            if (!result.Success && result.Message.Contains("tìm thấy"))
                return NotFound(result);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // POST /api/students/{studentId}/evaluations
        [HttpPost]
        [Authorize(Roles = "Admin,Staff,Teacher")]
        public async Task<IActionResult> Create(int studentId, [FromBody] CreateEvaluationRequest request)
        {
            var evaluatedBy = GetCurrentUserId();
            var result = await _service.CreateAsync(studentId, evaluatedBy, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // PUT /api/students/{studentId}/evaluations/{evaluationId}
        [HttpPut("{evaluationId:int}")]
        [Authorize(Roles = "Admin,Staff,Teacher")]
        public async Task<IActionResult> Update(
            int studentId, int evaluationId, [FromBody] UpdateEvaluationRequest request)
        {
            var result = await _service.UpdateAsync(evaluationId, request);
            if (!result.Success && result.Message.Contains("tìm thấy"))
                return NotFound(result);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        private int GetCurrentUserId()
        {
            var claim = User.FindFirst("userid") ?? User.FindFirst(ClaimTypes.NameIdentifier);
            return int.TryParse(claim?.Value, out var id) ? id : 0;
        }
    }
}
