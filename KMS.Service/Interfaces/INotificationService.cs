using KMS.Service.DTOs.Notification;
using KMS.Service.DTOs.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.Interfaces
{
    public interface INotificationService
    {
        Task<BaseResponseDTO> GetUserNotificationsAsync(int userId);

        Task<BaseResponseDTO> GetByIdAsync(int id);

        Task<BaseResponseDTO> CreateAsync(CreateNotificationDTO dto);

        Task<BaseResponseDTO> UpdateAsync(int id, UpdateNotificationDTO dto);

        Task<BaseResponseDTO> MarkAsReadAsync(int id);

        Task<BaseResponseDTO> DeleteAsync(int id);
        Task<BaseResponseDTO> GetStudentNotificationsAsync(int studentId);
    }
}
