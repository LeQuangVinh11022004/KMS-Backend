using KMS.Service.DTOs.Evaluation;
using KMS.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KMS.API.Controllers
{
    [ApiController]
    [Route("api/evaluation-criteria")]
    [Authorize]
    public class EvaluationCriteriaController : ControllerBase
    {
        private readonly IEvaluationCriteriaService _service;

        public EvaluationCriteriaController(IEvaluationCriteriaService service)
        {
            _service = service;
        }

        // GET /api/evaluation-criteria?activeOnly=true
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool activeOnly = true)
        {
            var result = await _service.GetAllAsync(activeOnly);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // POST /api/evaluation-criteria
        [HttpPost]
        [Authorize(Roles = "Admin,Staff,Teacher")]
        public async Task<IActionResult> Create([FromBody] CreateCriteriaRequest request)
        {
            var result = await _service.CreateAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // PATCH /api/evaluation-criteria/{criteriaId}/toggle
        [HttpPatch("{criteriaId:int}/toggle")]
        [Authorize(Roles = "Admin,Staff,Teacher")]
        public async Task<IActionResult> Toggle(int criteriaId, [FromBody] ToggleCriteriaRequest request)
        {
            var result = await _service.ToggleActiveAsync(criteriaId, request.IsActive);
            if (!result.Success && result.Message.Contains("tìm thấy"))
                return NotFound(result);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // DELETE /api/evaluation-criteria/{criteriaId}
        [HttpDelete("{criteriaId:int}")]
        [Authorize(Roles = "Admin,Staff,Teacher")]
        public async Task<IActionResult> Delete(int criteriaId)
        {
            var result = await _service.DeleteAsync(criteriaId);
            if (!result.Success && result.Message.Contains("tìm thấy"))
                return NotFound(result);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
