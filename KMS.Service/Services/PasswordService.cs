using KMS.Repository.Entities;
using KMS.Repository.Interfaces;
using KMS.Service.DTOs;
using KMS.Service.DTOs.ResetPassword;
using KMS.Service.DTOs.Role;
using KMS.Service.Interfaces;

namespace KMS.Service.Services
{
    public class PasswordService : IPasswordService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordResetTokenRepository _tokenRepository;
        private readonly IEmailService _emailService;

        private const int TokenExpiryMinutes = 30;

        public PasswordService(
            IUserRepository userRepository,
            IPasswordResetTokenRepository tokenRepository,
            IEmailService emailService)
        {
            _userRepository = userRepository;
            _tokenRepository = tokenRepository;
            _emailService = emailService;
        }

        // ADMIN - Reset password → trả về mật khẩu tạm

        public async Task<BaseResponseDTO> AdminResetPasswordAsync(int targetUserId, int adminId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(targetUserId);
                if (user == null)
                    return new BaseResponseDTO { Success = false, Message = "User not found" };

                var tempPassword = $"Temp@{new Random().Next(100000, 999999)}";
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(tempPassword);
                user.PasswordChangedAt = null;
                user.UpdatedAt = DateTime.Now;

                await _userRepository.UpdateAsync(user);

                await _tokenRepository.InvalidateAllUserTokensAsync(targetUserId);

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = "Password reset successfully",
                    Data = new AdminResetPasswordResultDTO
                    {
                        UserId = user.UserId,
                        Username = user.Username,
                        FullName = user.FullName,
                        TempPassword = tempPassword
                    }
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        // USER - Tự đổi mật khẩu (cần đăng nhập)

        public async Task<BaseResponseDTO> ChangePasswordAsync(int userId, ChangePasswordDTO dto)
        {
            try
            {
                if (dto.NewPassword != dto.ConfirmPassword)
                    return new BaseResponseDTO { Success = false, Message = "New password and confirm password do not match" };

                if (dto.NewPassword.Length < 6)
                    return new BaseResponseDTO { Success = false, Message = "New password must be at least 6 characters" };

                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                    return new BaseResponseDTO { Success = false, Message = "User not found" };

                if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
                    return new BaseResponseDTO { Success = false, Message = "Current password is incorrect" };

                if (BCrypt.Net.BCrypt.Verify(dto.NewPassword, user.PasswordHash))
                    return new BaseResponseDTO { Success = false, Message = "New password must be different from current password" };

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
                user.PasswordChangedAt = DateTime.Now;
                user.UpdatedAt = DateTime.Now;

                await _userRepository.UpdateAsync(user);

                return new BaseResponseDTO { Success = true, Message = "Password changed successfully" };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        // FORGOT PASSWORD - Tạo token (AllowAnonymous)

        public async Task<BaseResponseDTO> ForgotPasswordAsync(ForgotPasswordDTO dto)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(dto.Email);

                if (user == null || user.IsActive != true)
                    return new BaseResponseDTO
                    {
                        Success = true,
                        Message = "If this email exists, a reset token has been generated."
                    };

                await _tokenRepository.InvalidateAllUserTokensAsync(user.UserId);

                var rawToken = new Random().Next(100000, 999999).ToString();
                var expiresAt = DateTime.Now.AddMinutes(TokenExpiryMinutes);

                var tokenEntity = new KmsPasswordResetToken
                {
                    UserId = user.UserId,
                    Token = rawToken,
                    ExpiresAt = expiresAt,
                    IsUsed = false,
                    CreatedAt = DateTime.Now
                };

                await _tokenRepository.CreateAsync(tokenEntity);

                var emailSent = false;
                try
                {
                    await _emailService.SendResetPasswordEmailAsync(
                        user.Email!, user.FullName, rawToken, expiresAt);
                    emailSent = true;
                }
                catch
                {
                    emailSent = false;
                }

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = emailSent
                        ? $"Email đặt lại mật khẩu đã được gửi. Token hết hạn sau {TokenExpiryMinutes} phút."
                        : $"Không thể gửi email. Token hết hạn sau {TokenExpiryMinutes} phút.",
                    Data = new ForgotPasswordResultDTO
                    {
                        Message = emailSent ? "Kiểm tra hộp thư email của bạn" : "Gửi email thất bại - liên hệ Admin",
                        ResetToken = emailSent ? "[Đã gửi qua email]" : rawToken,
                        ExpiresAt = expiresAt
                    }
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        // RESET PASSWORD WITH TOKEN (AllowAnonymous)

        public async Task<BaseResponseDTO> ResetPasswordWithTokenAsync(ResetPasswordDTO dto)
        {
            try
            {
                if (dto.NewPassword != dto.ConfirmPassword)
                    return new BaseResponseDTO { Success = false, Message = "Passwords do not match" };

                if (dto.NewPassword.Length < 6)
                    return new BaseResponseDTO { Success = false, Message = "Password must be at least 6 characters" };

                var tokenEntity = await _tokenRepository.GetByTokenAsync(dto.Token);

                if (tokenEntity == null)
                    return new BaseResponseDTO { Success = false, Message = "Invalid or expired token" };

                if (tokenEntity.IsUsed == true)
                    return new BaseResponseDTO { Success = false, Message = "Token has already been used" };

                if (tokenEntity.ExpiresAt < DateTime.Now)
                    return new BaseResponseDTO { Success = false, Message = $"Token has expired. Please request a new one." };

                var user = tokenEntity.User;
                if (user == null || user.IsActive != true)
                    return new BaseResponseDTO { Success = false, Message = "User not found or inactive" };

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
                user.PasswordChangedAt = DateTime.Now;
                user.UpdatedAt = DateTime.Now;

                await _userRepository.UpdateAsync(user);

                await _tokenRepository.MarkAsUsedAsync(tokenEntity.TokenId);

                return new BaseResponseDTO { Success = true, Message = "Password reset successfully. You can now login with your new password." };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }
    }
}