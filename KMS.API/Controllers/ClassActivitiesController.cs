using KMS.Service.DTOs.ActivityMedia;
using KMS.Service.DTOs.ClassActivity;
using KMS.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ClassActivitiesController : ControllerBase
    {
        private readonly IClassActivityService _service;
        private readonly IActivityMediaService _photoService;
        private readonly IWebHostEnvironment _env;
        public ClassActivitiesController(IClassActivityService service, IActivityMediaService photoService, IWebHostEnvironment env)
        {
            _service = service;
            _photoService = photoService;
            _env = env;
        }

        private string GetBaseUrl()
        {
            return $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
        }
        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : 0;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("class/{classId}")]
        public async Task<IActionResult> GetByClass(int classId)
        {
            return Ok(await _service.GetByClassIdAsync(classId));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Teacher,Staff")]
        public async Task<IActionResult> Create(ClassActivityDTO dto)
        {
            await _service.CreateAsync(dto, GetUserId());
            return Ok("Created successfully");
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Teacher,Staff")]
        public async Task<IActionResult> Update(int id, ClassActivityDTO dto)
        {
            await _service.UpdateAsync(id, dto, GetUserId());
            return Ok("Updated successfully");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Teacher,Staff")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return Ok("Deleted successfully");
        }

        /// photo endpoints
        [HttpGet("{activityId}/photos")]
        public async Task<IActionResult> GetPhotos(int activityId)
        {
            return Ok(await _photoService.GetByActivityIdAsync(activityId, GetBaseUrl()));
        }

        [HttpPost("{activityId}/photos")]
        public async Task<IActionResult> AddPhoto(int activityId, [FromForm] ActivityMediaCreateDTO dto)
        {
            var webRootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            var result = await _photoService.AddAsync(activityId, dto, webRootPath, GetBaseUrl());
            return Ok(new
            {
                message = "Photo added successfully",
                data = result
            });
        }

        [HttpDelete("photos/{photoId}")]
        public async Task<IActionResult> DeletePhoto(int photoId)
        {
            await _photoService.DeleteAsync(photoId);
            return Ok("Photo deleted successfully");
        }
    }
}
