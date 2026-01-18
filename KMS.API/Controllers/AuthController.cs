using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KMS.Service.DTOs;
using KMS.Service.Interfaces;
using System.Security.Claims;

namespace KMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Login endpoint
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid request data",
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }

            var result = await _authService.LoginAsync(request);
            if (!result.Success)
            {
                return Unauthorized(new
                {
                    success = false,
                    message = result.Message
                });
            }

            _logger.LogInformation($"User {request.Username} logged in successfully at {DateTime.Now}");

            return Ok(new
            {
                success = true,
                message = result.Message,
                data = new
                {
                    token = result.Token,
                    user = result.User
                }
            });
        }

        /// <summary>
        /// Register new user endpoint
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid request data",
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }

            var result = await _authService.RegisterAsync(request);

            if (!result.Success)
            {
                return BadRequest(new
                {
                    success = false,
                    message = result.Message
                });
            }

            _logger.LogInformation($"User {request.Username} registered successfully at {DateTime.Now}");

            return Ok(new
            {
                success = true,
                message = result.Message
            });
        }

        /// <summary>
        /// Get current user profile (requires authentication)
        /// </summary>
        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "Invalid token"
                });
            }

            var user = await _authService.GetUserByIdAsync(userId);

            if (user == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "User not found"
                });
            }

            return Ok(new
            {
                success = true,
                data = user
            });
        }

        /// <summary>
        /// Test endpoint - Only Admin can access
        /// </summary>
        [HttpGet("admin-only")]
        [Authorize(Roles = "Admin")]
        public IActionResult AdminOnly()
        {
            return Ok(new
            {
                success = true,
                message = "Welcome Admin!",
                user = User.FindFirst(ClaimTypes.Name)?.Value,
                roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList()
            });
        }

        /// <summary>
        /// Test endpoint - Admin or Teacher can access
        /// </summary>
        [HttpGet("teacher-access")]
        [Authorize(Roles = "Admin,Teacher")]
        public IActionResult TeacherAccess()
        {
            var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

            return Ok(new
            {
                success = true,
                message = "Welcome!",
                user = User.FindFirst(ClaimTypes.Name)?.Value,
                roles = roles
            });
        }

        /// <summary>
        /// Test endpoint - Any authenticated user
        /// </summary>
        [HttpGet("authenticated")]
        [Authorize]
        public IActionResult Authenticated()
        {
            var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

            return Ok(new
            {
                success = true,
                message = "You are authenticated",
                user = User.FindFirst(ClaimTypes.Name)?.Value,
                userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                roles = roles
            });
        }
    }
}