using KMS.Service.DTOs;
using KMS.Service.DTOs.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.Services
{
    public interface IParentRegistrationService
    {
        // Public - phụ huynh tự submit
        Task<BaseResponseDTO> SubmitRegistrationAsync(CreateParentRegistrationDTO dto);

        // Admin - xem danh sách
        Task<BaseResponseDTO> GetAllRegistrationsAsync();
        Task<BaseResponseDTO> GetRegistrationByIdAsync(int registrationId);
        Task<BaseResponseDTO> GetPendingRegistrationsAsync();

        // Admin - duyệt hoặc từ chối
        Task<BaseResponseDTO> ApproveRegistrationAsync(int registrationId, int reviewedBy, ReviewRegistrationDTO dto);
        Task<BaseResponseDTO> RejectRegistrationAsync(int registrationId, int reviewedBy, ReviewRegistrationDTO dto);
    }
}
