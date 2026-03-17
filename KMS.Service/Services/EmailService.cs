using KMS.Service.Interfaces;
using KMS.Service.Settings;
//using System.Net.Mail;
using MailKit.Net.Smtp;
using MailKit.Security;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit;
using System.Net;
using System.Text;
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
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(
                _settings.SenderName,
                _settings.SenderEmail));

            message.To.Add(MailboxAddress.Parse(toEmail));

            message.Subject = subject;

            message.Body = new TextPart("html")
            {
                Text = htmlBody
            };

            using var client = new SmtpClient();

            await client.ConnectAsync(
                _settings.SmtpHost,
                _settings.SmtpPort,
                SecureSocketOptions.StartTls);

            await client.AuthenticateAsync(
                _settings.SenderEmail,
                _settings.AppPassword);

            await client.SendAsync(message);

            await client.DisconnectAsync(true);
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
        // ============================================================
        // ATTENDANCE NOTIFICATION - Phụ huynh
        // ============================================================

        public async Task SendAttendanceAlertEmailAsync(
            string toEmail,
            string parentName,
            string studentName,
            string status,
            DateTime date)
        {
            var subject = $"[{_settings.SenderName}] Thông báo điểm danh của {studentName}";

            var statusText = status == "Absent"
                ? "vắng mặt"
                : "đến lớp trễ";

                    var body = $@"
        <!DOCTYPE html>
        <html>
        <head>
        <meta charset='utf-8'>
        <style>
        body {{ font-family: Arial; background:#f5f5f5; padding:20px; }}
        .container {{ max-width:600px; margin:auto; background:white; border-radius:8px; overflow:hidden; }}
        .header {{ background:#ff9800; padding:20px; text-align:center; color:white; }}
        .content {{ padding:25px; }}
        .info {{ background:#fff3cd; padding:15px; border-radius:6px; margin:15px 0; }}
        .footer {{ background:#f5f5f5; padding:15px; text-align:center; font-size:12px; color:#888; }}
        </style>
        </head>
        <body>

        <div class='container'>

        <div class='header'>
        <h2>📢 Thông báo điểm danh</h2>
        </div>

        <div class='content'>

        <p>Xin chào <strong>{parentName}</strong>,</p>

        <p>Nhà trường xin thông báo:</p>

        <div class='info'>
        👶 Học sinh: <strong>{studentName}</strong><br/>
        📅 Ngày: <strong>{date:dd/MM/yyyy}</strong><br/>
        ⚠️ Trạng thái: <strong>{statusText}</strong>
        </div>

        <p>Nếu có thắc mắc vui lòng liên hệ nhà trường.</p>

        <p>Trân trọng,<br/><strong>{_settings.SenderName}</strong></p>

        </div>

        <div class='footer'>
        Email được gửi tự động từ hệ thống quản lý trường học.
        </div>

        </div>

        </body>
        </html>";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendInvoiceEmailAsync(
    string toEmail,
    string parentName,
    string studentName,
    string invoiceNumber,
    DateTime monthYear,
    List<(string ItemName, decimal Amount)> items,
    decimal totalAmount,
    decimal discountAmount,
    decimal finalAmount,
    string vnpTransactionNo,
    string orderId,
    string bankCode,
    DateTime paymentDate)
        {
            var subject = $"[{_settings.SenderName}] Xác nhận thanh toán hóa đơn #{invoiceNumber}";

            // Build các dòng khoản phí
            var itemRows = new StringBuilder();
            foreach (var item in items)
            {
                itemRows.Append($@"
          <tr>
            <td style='padding:10px 8px; border-bottom:1px solid #eee; font-size:14px;'>{item.ItemName}</td>
            <td style='padding:10px 8px; border-bottom:1px solid #eee; font-size:14px; text-align:right;'>
              {item.Amount:N0} VNĐ
            </td>
          </tr>");
            }

            var discountRow = discountAmount > 0 ? $@"
          <tr>
            <td style='padding:10px 8px; border-bottom:1px solid #eee; font-size:14px; color:#e53935;'>
              🎁 Giảm giá
            </td>
            <td style='padding:10px 8px; border-bottom:1px solid #eee; font-size:14px; 
                       text-align:right; color:#e53935;'>
              - {discountAmount:N0} VNĐ
            </td>
          </tr>" : "";

            var body = $@"
<!DOCTYPE html>
<html>
<head><meta charset='utf-8'></head>
<body style='font-family:Arial,sans-serif; background:#f5f5f5; margin:0; padding:20px;'>
  <div style='max-width:600px; margin:0 auto; background:white; border-radius:8px; 
              overflow:hidden; box-shadow:0 2px 8px rgba(0,0,0,0.1);'>

    <!-- HEADER -->
    <div style='background:#4CAF50; padding:30px; text-align:center;'>
      <h1 style='color:white; margin:0; font-size:22px;'>✅ Thanh toán thành công!</h1>
    </div>

    <!-- BODY -->
    <div style='padding:30px;'>
      <p>Xin chào <strong>{parentName}</strong>,</p>
      <p>Hệ thống đã ghi nhận thanh toán học phí tháng 
         <strong>{monthYear:MM/yyyy}</strong> cho học sinh 
         <strong>{studentName}</strong>.</p>

      <!-- CHI TIẾT KHOẢN PHÍ -->
      <p style='font-weight:bold; color:#333; margin-bottom:8px;'>📋 Chi tiết hóa đơn:</p>
      <table style='width:100%; border-collapse:collapse; 
                    border:1px solid #e0e0e0; border-radius:8px; overflow:hidden;'>
        <thead>
          <tr style='background:#f5f5f5;'>
            <th style='padding:10px 8px; text-align:left; font-size:14px; color:#666;'>Khoản phí</th>
            <th style='padding:10px 8px; text-align:right; font-size:14px; color:#666;'>Số tiền</th>
          </tr>
        </thead>
        <tbody>
          {itemRows}
          {discountRow}
          <tr style='background:#e8f5e9;'>
            <td style='padding:12px 8px; font-weight:bold; font-size:15px; color:#2e7d32;'>
              💰 Tổng thanh toán
            </td>
            <td style='padding:12px 8px; font-weight:bold; font-size:15px; 
                       text-align:right; color:#2e7d32;'>
              {finalAmount:N0} VNĐ
            </td>
          </tr>
        </tbody>
      </table>

      <!-- THÔNG TIN GIAO DỊCH -->
      <p style='font-weight:bold; color:#333; margin:20px 0 8px;'>🏦 Thông tin giao dịch:</p>
      <table style='width:100%; border-collapse:collapse; 
                    background:#f9f9f9; border-radius:8px; border:1px solid #e0e0e0;'>
        <tr>
          <td style='padding:10px 12px; font-size:14px; color:#666; width:45%;'>📅 Ngày thanh toán</td>
          <td style='padding:10px 12px; font-size:14px; font-weight:bold;'>{paymentDate:dd/MM/yyyy HH:mm}</td>
        </tr>
        <tr style='background:#f0f0f0;'>
          <td style='padding:10px 12px; font-size:14px; color:#666;'>🏦 Ngân hàng</td>
          <td style='padding:10px 12px; font-size:14px; font-weight:bold;'>{bankCode}</td>
        </tr>
        <tr>
          <td style='padding:10px 12px; font-size:14px; color:#666;'>🔖 Mã đơn hàng</td>
          <td style='padding:10px 12px; font-size:14px; font-weight:bold;'>{orderId}</td>
        </tr>
        <tr style='background:#f0f0f0;'>
          <td style='padding:10px 12px; font-size:14px; color:#666;'>🔗 Mã giao dịch VNPay</td>
          <td style='padding:10px 12px; font-size:14px; font-weight:bold;'>{vnpTransactionNo}</td>
        </tr>
        <tr>
          <td style='padding:10px 12px; font-size:14px; color:#666;'>📊 Trạng thái</td>
          <td style='padding:10px 12px;'>
            <span style='background:#4CAF50; color:white; padding:3px 12px; 
                         border-radius:20px; font-size:13px;'>Đã thanh toán</span>
          </td>
        </tr>
      </table>

      <p style='margin-top:24px;'>Vui lòng lưu email này làm biên lai. 
         Nếu có thắc mắc, hãy liên hệ nhà trường.</p>
      <p>Trân trọng,<br><strong>{_settings.SenderName}</strong></p>
    </div>

    <!-- FOOTER -->
    <div style='background:#f5f5f5; padding:15px 30px; text-align:center; 
                font-size:13px; color:#888;'>
      Email này được gửi tự động, vui lòng không reply.
    </div>
  </div>
</body>
</html>";

            await SendEmailAsync(toEmail, subject, body);
        }
    }
}