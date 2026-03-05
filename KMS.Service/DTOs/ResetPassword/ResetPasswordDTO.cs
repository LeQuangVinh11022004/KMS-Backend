using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.DTOs.ResetPassword
{
    public class AdminResetPasswordResultDTO
    {
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string TempPassword { get; set; } = null!;
        public string Note { get; set; } = "Vui lòng gửi mật khẩu tạm này cho người dùng và yêu cầu đổi mật khẩu sau khi đăng nhập.";
    }

    public class ChangePasswordDTO
    {
        public string CurrentPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
        public string ConfirmPassword { get; set; } = null!;
    }

    // ── Forgot Password Flow ──────────────────────────────────────────

    public class ForgotPasswordDTO
    {
        public string Email { get; set; } = null!;
    }

    public class ForgotPasswordResultDTO
    {
        public string Message { get; set; } = null!;
        // TODO: Xóa ResetToken khỏi response sau khi gắn EmailService
        // Hiện tại trả về để Admin copy gửi cho user qua Zalo/SMS
        public string ResetToken { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
    }

    public class ResetPasswordDTO
    {
        public string Token { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
        public string ConfirmPassword { get; set; } = null!;
    }
}
