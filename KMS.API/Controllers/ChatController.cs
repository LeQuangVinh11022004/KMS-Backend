using KMS.Service.DTOs.Chat;
using KMS.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KMS.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Parent")]
    public class ChatController : ControllerBase
    {
        private readonly IChatAiService _chatService;

        public ChatController(IChatAiService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] ChatRequestDTO request)
        {
            // Lấy UserId từ JWT Token của người đang đăng nhập
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var result = await _chatService.GetAiResponseAsync(userId, request);
            return Ok(result);
        }
    }
}
