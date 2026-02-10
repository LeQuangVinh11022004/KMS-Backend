using KMS.Service.DTOs.SchoolYear;
using KMS.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KMS.API.Controllers
{
    [ApiController]
    [Route("api/school-years")]
    public class SchoolYearsController : ControllerBase
    {
        private readonly ISchoolYearService _service;

        public SchoolYearsController(ISchoolYearService service)
        {
            _service = service;
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
        public async Task<IActionResult> Create(SchoolYearCreateDTO dto)
        {
            await _service.CreateAsync(dto, createdBy: 1);
            return Ok("SchoolYear created");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, SchoolYearUpdateDTO dto)
        {
            var ok = await _service.UpdateAsync(id, dto, updatedBy: 1);
            if (!ok) return NotFound();
            return Ok("SchoolYear updated");
        }
    }
}
