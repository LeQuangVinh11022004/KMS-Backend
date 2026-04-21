using KMS.Service.DTOs.SchoolYear;
using KMS.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KMS.API.Controllers
{
    [ApiController]
    [Route("api/school-years")]
    [Authorize]
    public class SchoolYearsController : ControllerBase
    {
        private readonly ISchoolYearService _service;

        public SchoolYearsController(ISchoolYearService service)
        {
            _service = service;
        }
        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : 0;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _service.GetByIdAsync(id);
            if (data == null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Create(SchoolYearCreateDTO dto)
        {
            await _service.CreateAsync(dto, GetUserId());
            return Ok("SchoolYear created");
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Update(int id, SchoolYearUpdateDTO dto)
        {
            var ok = await _service.UpdateAsync(id, dto, GetUserId());
            if (!ok) return NotFound();
            return Ok("SchoolYear updated");
        }
    }
}
