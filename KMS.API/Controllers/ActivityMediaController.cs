using KMS.Service.DTOs.ActivityMedia;
using KMS.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KMS.API.Controllers
{
    [Route("api/activity-media")]
    [ApiController]
    public class ActivityMediaController : ControllerBase
    {
        private readonly IActivityMediaService _service;

        public ActivityMediaController(IActivityMediaService service)
        {
            _service = service;
        }

        // get media by activity

        [HttpGet("activity/{activityId}")]
        public async Task<IActionResult> GetByActivity(int activityId)
        {
            var result = await _service.GetByActivityIdAsync(activityId);

            return Ok(result);
        }

        // Add media

        [HttpPost("activity/{activityId}")]
        public async Task<IActionResult> AddMedia(int activityId, [FromBody] ActivityMediaDTO dto)
        {
            await _service.AddAsync(activityId, dto);

            return Ok(new
            {
                message = "Media added successfully"
            });
        }

        // delete media

        [HttpDelete("{mediaId}")]
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