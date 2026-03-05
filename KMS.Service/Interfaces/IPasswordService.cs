using KMS.Service.DTOs.ResetPassword;
using KMS.Service.DTOs.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.Interfaces
{
    public interface IPasswordService
    {
        // Admin reset password cho user bất kỳ → trả về mật khẩu tạm
        Task<BaseResponseDTO> AdminResetPasswordAsync(int targetUserId, int adminId);

        // User tự đổi mật khẩu của mình (cần đăng nhập)
        Task<BaseResponseDTO> ChangePasswordAsync(int userId, ChangePasswordDTO dto);

        // User quên mật khẩu → tạo token (sau này gắn email)
        Task<BaseResponseDTO> ForgotPasswordAsync(ForgotPasswordDTO dto);

        // User dùng token để đặt lại mật khẩu
        Task<BaseResponseDTO> ResetPasswordWithTokenAsync(ResetPasswordDTO dto);
    }
}
