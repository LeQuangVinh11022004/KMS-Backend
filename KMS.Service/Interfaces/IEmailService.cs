using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.Interfaces
{
    public interface IEmailService
    {
        // Gửi email tài khoản sau khi Admin approve ParentRegistration
        Task SendAccountCreatedEmailAsync(string toEmail, string fullName, string username, string tempPassword);

        // Gửi email reset password token
        Task SendResetPasswordEmailAsync(string toEmail, string fullName, string resetToken, DateTime expiresAt);

        // Generic - gửi email bất kỳ
        Task SendEmailAsync(string toEmail, string subject, string htmlBody);

        Task SendAttendanceAlertEmailAsync(string toEmail, string parentName, string studentName, string status, DateTime date);
    }
}
