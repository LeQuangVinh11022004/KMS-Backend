using KMS.Service.Interfaces;
using KMS.Service.Settings;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace KMS.Service.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        // ============================================================
        // GENERIC SEND
        // ============================================================

        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            using var client = new SmtpClient(_settings.SmtpHost, _settings.SmtpPort)
            {
                Credentials = new NetworkCredential(_settings.SenderEmail, _settings.AppPassword),
                EnableSsl = true
            };

            var mail = new MailMessage
            {
                From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };
            mail.To.Add(toEmail);

            await client.SendMailAsync(mail);
        }

        // ============================================================
        // TÀI KHOẢN MỚI - Sau khi Admin approve ParentRegistration
        // ============================================================

        public async Task SendAccountCreatedEmailAsync(
            string toEmail, string fullName, string username, string tempPassword)
        {
            var subject = $"[{_settings.SenderName}] Tài khoản của bạn đã được tạo";

            var body = $@"
<!DOCTYPE html>
<html>
<head>
  <meta charset='utf-8'>
  <style>
    body {{ font-family: Arial, sans-serif; background: #f5f5f5; margin: 0; padding: 20px; }}
    .container {{ max-width: 600px; margin: 0 auto; background: white; border-radius: 8px; overflow: hidden; box-shadow: 0 2px 8px rgba(0,0,0,0.1); }}
    .header {{ background: #4CAF50; padding: 30px; text-align: center; }}
    .header h1 {{ color: white; margin: 0; font-size: 24px; }}
    .body {{ padding: 30px; }}
    .info-box {{ background: #f9f9f9; border-left: 4px solid #4CAF50; padding: 15px 20px; margin: 20px 0; border-radius: 4px; }}
    .info-box p {{ margin: 8px 0; font-size: 15px; }}
    .info-box strong {{ color: #333; }}
    .warning {{ background: #fff3cd; border: 1px solid #ffc107; padding: 12px 16px; border-radius: 4px; margin: 20px 0; font-size: 14px; color: #856404; }}
    .footer {{ background: #f5f5f5; padding: 15px 30px; text-align: center; font-size: 13px; color: #888; }}
  </style>
</head>
<body>
  <div class='container'>
    <div class='header'>
      <h1>🎉 Chào mừng đến với {_settings.SenderName}!</h1>
    </div>
    <div class='body'>
      <p>Xin chào <strong>{fullName}</strong>,</p>
      <p>Đơn đăng ký của bạn đã được nhà trường xét duyệt và chấp thuận. Dưới đây là thông tin đăng nhập của bạn:</p>

      <div class='info-box'>
        <p>🔑 <strong>Tên đăng nhập:</strong> {username}</p>
        <p>🔒 <strong>Mật khẩu tạm:</strong> {tempPassword}</p>
      </div>

      <div class='warning'>
        ⚠️ Đây là mật khẩu tạm thời. Vui lòng đăng nhập và <strong>đổi mật khẩu ngay</strong> sau lần đăng nhập đầu tiên để bảo mật tài khoản.
      </div>

      <p>Nếu bạn cần hỗ trợ, vui lòng liên hệ nhà trường.</p>
      <p>Trân trọng,<br><strong>{_settings.SenderName}</strong></p>
    </div>
    <div class='footer'>
      Email này được gửi tự động, vui lòng không reply.
    </div>
  </div>
</body>
</html>";

            await SendEmailAsync(toEmail, subject, body);
        }

        // ============================================================
        // RESET PASSWORD - Gửi token khi quên mật khẩu
        // ============================================================

        public async Task SendResetPasswordEmailAsync(
            string toEmail, string fullName, string resetToken, DateTime expiresAt)
        {
            var subject = $"[{_settings.SenderName}] Yêu cầu đặt lại mật khẩu";

            var body = $@"
<!DOCTYPE html>
<html>
<head>
  <meta charset='utf-8'>
  <style>
    body {{ font-family: Arial, sans-serif; background: #f5f5f5; margin: 0; padding: 20px; }}
    .container {{ max-width: 600px; margin: 0 auto; background: white; border-radius: 8px; overflow: hidden; box-shadow: 0 2px 8px rgba(0,0,0,0.1); }}
    .header {{ background: #2196F3; padding: 30px; text-align: center; }}
    .header h1 {{ color: white; margin: 0; font-size: 24px; }}
    .body {{ padding: 30px; }}
    .token-box {{ background: #e3f2fd; border: 2px dashed #2196F3; padding: 20px; margin: 20px 0; border-radius: 8px; text-align: center; }}
    .token {{ font-size: 22px; font-weight: bold; letter-spacing: 4px; color: #1565C0; font-family: monospace; word-break: break-all; }}
    .expire {{ font-size: 13px; color: #888; margin-top: 8px; }}
    .warning {{ background: #fff3cd; border: 1px solid #ffc107; padding: 12px 16px; border-radius: 4px; margin: 20px 0; font-size: 14px; color: #856404; }}
    .footer {{ background: #f5f5f5; padding: 15px 30px; text-align: center; font-size: 13px; color: #888; }}
  </style>
</head>
<body>
  <div class='container'>
    <div class='header'>
      <h1>🔐 Đặt lại mật khẩu</h1>
    </div>
    <div class='body'>
      <p>Xin chào <strong>{fullName}</strong>,</p>
      <p>Chúng tôi nhận được yêu cầu đặt lại mật khẩu cho tài khoản của bạn. Nhập mã OTP dưới đây để đặt lại mật khẩu:</p>

      <div class='token-box'>
        <div class='token'>{resetToken}</div>
        <div class='expire'>⏱ Hết hạn lúc: {expiresAt:dd/MM/yyyy HH:mm}</div>
      </div>

      <div class='warning'>
        ⚠️ Mã OTP chỉ có hiệu lực trong <strong>30 phút</strong> và chỉ dùng được <strong>1 lần</strong>. Nếu bạn không yêu cầu đặt lại mật khẩu, hãy bỏ qua email này.
      </div>

      <p>Trân trọng,<br><strong>{_settings.SenderName}</strong></p>
    </div>
    <div class='footer'>
      Email này được gửi tự động, vui lòng không reply.
    </div>
  </div>
</body>
</html>";

            await SendEmailAsync(toEmail, subject, body);
        }
    }
}