using KMS.Service.DTOs;
using KMS.Service.DTOs.Role;
using KMS.Service.DTOs.VNPay;
using KMS.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VNPayController : ControllerBase
    {
        private readonly IVNPayService _vnpayService;

        public VNPayController(IVNPayService vnpayService)
        {
            _vnpayService = vnpayService;
        }

        /// <summary>
        /// Create VNPay payment URL
        /// </summary>
        [HttpPost("create-payment")]
        [Authorize(Roles = "Parent,Admin,Staff")]
        public async Task<IActionResult> CreatePayment([FromBody] CreateVNPayPaymentDTO dto)
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized(new BaseResponseDTO { Success = false, Message = "Invalid token" });

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
            if (ipAddress == "::1") ipAddress = "127.0.0.1";

            var result = await _vnpayService.CreatePaymentUrlAsync(dto, userId, ipAddress);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// VNPay callback after payment
        /// </summary>
        [HttpGet("return")]
        [AllowAnonymous]
        public async Task<IActionResult> Return()
        {
            var result = await _vnpayService.ProcessCallbackAsync(Request.Query);
            var queryString = Request.QueryString.HasValue ? Request.QueryString.Value : "";

            var userAgent = Request.Headers["User-Agent"].ToString();
            var isMobile = !string.IsNullOrEmpty(userAgent) &&
                          (userAgent.Contains("Mobile") || userAgent.Contains("Android") || userAgent.Contains("iOS"));

            var frontendUrl = result.Success
                ? (isMobile ? $"kmsmobile://payment/success{queryString}" : $"https://mobile-cap.vercel.app/success{queryString}")
                : (isMobile ? $"kmsmobile://payment/failed{queryString}" : $"https://mobile-cap.vercel.app/failed{queryString}");

            return Redirect(frontendUrl);
        }

        /// <summary>
        /// Return vnpay by json
        /// </summary>
        [HttpGet("return-json")]
        [AllowAnonymous]
        public async Task<IActionResult> ReturnJson()
        {
            try
            {
                var allParams = Request.Query.ToDictionary(q => q.Key, q => q.Value.ToString());
                Console.WriteLine($"[VNPay] ReturnJson received params: {string.Join(", ", allParams.Select(p => $"{p.Key}={p.Value}"))}");

                var result = await _vnpayService.ProcessCallbackAsync(Request.Query);

                if (!result.Success)
                {
                    Console.WriteLine($"[VNPay] ReturnJson failed: {result.Message}");
                    return BadRequest(result);
                }

                Console.WriteLine($"[VNPay] ReturnJson success: {result.Message}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[VNPay] ReturnJson exception: {ex.Message}");
                return BadRequest(new BaseResponseDTO { Success = false, Message = $"Internal error: {ex.Message}" });
            }
        }

        private int GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out int id) ? id : 0;
        }
    }
}