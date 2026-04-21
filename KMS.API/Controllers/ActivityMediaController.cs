using KMS.Service.DTOs.ActivityMedia;
using KMS.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace KMS.API.Controllers
{
    [Route("api/activity-media")]
    [ApiController]
    public class ActivityMediaController : ControllerBase
    {
        private readonly IActivityMediaService _service;
        private readonly IWebHostEnvironment _env;

        public ActivityMediaController(IActivityMediaService service, IWebHostEnvironment env)
        {
            _service = service;
                _env = env;
        }

        // get base url for media url
        private string GetBaseUrl()
        {
            return $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
        }

        // get media by activity
        [Authorize]
        [HttpGet("activity/{activityId}")]
        public async Task<IActionResult> GetByActivity(int activityId)
        {
            var result = await _service.GetByActivityIdAsync(activityId, GetBaseUrl());

            return Ok(result);
        }

        // Add media
        [Authorize(Roles = "Admin,Teacher,Staff")]
        [HttpPost("activity/{activityId}")]
        public async Task<IActionResult> AddMedia(int activityId, [FromForm] ActivityMediaCreateDTO dto)
        {
            var webRootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            var result = await _service.AddAsync(activityId, dto, webRootPath, GetBaseUrl());

            return Ok(new
            {
                message = "Media added successfully",
                data = result
            });
        }

        // delete media

        [HttpDelete("{mediaId}")]
        [Authorize(Roles = "Admin,Teacher,Staff")]
        public async Task<IActionResult> DeleteMedia(int mediaId)
        {
            await _service.DeleteAsync(mediaId);

            return Ok(new
            {
                message = "Media deleted successfully"
            });
        }
    }
}