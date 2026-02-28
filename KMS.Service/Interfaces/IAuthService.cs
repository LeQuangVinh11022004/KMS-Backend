using KMS.Service.DTOs.Login;
using KMS.Service.DTOs.Register;
using KMS.Service.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDTO> LoginAsync(LoginRequestDTO request);
        Task<LoginResponseDTO> RegisterAsync(RegisterRequestDTO request);
        Task<UserDTO?> GetUserByIdAsync(int userId);

    }
}
