using KMS.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KMS.API.Controllers
{
    [ApiController]
    [Route("api/classes/{classId:int}/evaluations")]
    [Authorize]
    public class ClassEvaluationController : ControllerBase
    {
        private readonly IEvaluationService _service;

        public ClassEvaluationController(IEvaluationService service)
        {
            _service = service;
        }

        // GET /api/classes/{classId}/evaluations/summary?periodType=monthly&periodStart=2026-05-01
        [HttpGet("summary")]
        public async Task<IActionResult> GetClassSummary(
            int classId,
            [FromQuery] string periodType,
            [FromQuery] DateOnly periodStart)
        {
            var result = await _service.GetClassSummaryAsync(classId, periodType, periodStart);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // GET /api/classes/{classId}/evaluations/good-students?periodType=monthly&periodStart=2026-05-01
        [HttpGet("good-students")]
        public async Task<IActionResult> GetGoodStudents(
            int classId,
            [FromQuery] string periodType,
            [FromQuery] DateOnly periodStart)
        {
            var result = await _service.GetGoodStudentsAsync(classId, periodType, periodStart);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
