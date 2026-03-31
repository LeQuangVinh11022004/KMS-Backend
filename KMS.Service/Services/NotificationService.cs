using KMS.Repository.Entities;
using KMS.Repository.Interfaces;
using KMS.Service.DTOs.Notification;
using KMS.Service.DTOs.Role;
using KMS.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repository;

        public NotificationService(INotificationRepository repository)
        {
            _repository = repository;
        }

        public async Task<BaseResponseDTO> GetUserNotificationsAsync(int userId)
        {
            var list = await _repository.GetByUserIdAsync(userId);

            var data = list.Select(n => Map(n));

            return new BaseResponseDTO
            {
                Success = true,
                Message = "Notifications retrieved",
                Data = data
            };
        }

        public async Task<BaseResponseDTO> GetByIdAsync(int id)
        {
            var notification = await _repository.GetByIdAsync(id);

            if (notification == null)
                return new BaseResponseDTO
                {
                    Success = false,
                    Message = "Notification not found"
                };

            return new BaseResponseDTO
            {
                Success = true,
                Data = Map(notification)
            };
        }

        public async Task<BaseResponseDTO> GetStudentNotificationsAsync(int studentId)
        {
            var list = await _repository.GetByStudentIdAsync(studentId);
            var data = list.Select(n => Map(n));
            return new BaseResponseDTO { Success = true, Message = "Student notifications retrieved", Data = data };
        }

        public async Task<BaseResponseDTO> CreateAsync(CreateNotificationDTO dto)
        {
            var entity = new KmsNotification
            {
                UserId = dto.UserId,
                StudentId = dto.StudentId,
                Type = dto.Type,
                Title = dto.Title,
                Message = dto.Message,
                RelatedEntityType = dto.RelatedEntityType,
                RelatedEntityId = dto.RelatedEntityId,
                IsRead = false,
                CreatedAt = DateTime.Now
            };

            await _repository.CreateAsync(entity);
            await _repository.UpdateAsync(entity);

            return new BaseResponseDTO
            {
                Success = true,
                Message = "Notification created",
                Data = Map(entity)
            };
        }

        public async Task<BaseResponseDTO> UpdateAsync(int id, UpdateNotificationDTO dto)
        {
            var notification = await _repository.GetByIdAsync(id);

            if (notification == null)
                return new BaseResponseDTO { Success = false, Message = "Notification not found" };

            notification.Title = dto.Title;
            notification.Message = dto.Message;

            await _repository.UpdateAsync(notification);

            return new BaseResponseDTO
            {
                Success = true,
                Message = "Notification updated"
            };
        }

        public async Task<BaseResponseDTO> MarkAsReadAsync(int id)
        {
            var notification = await _repository.GetByIdAsync(id);

            if (notification == null)
                return new BaseResponseDTO { Success = false, Message = "Notification not found" };

            notification.IsRead = true;
            notification.ReadAt = DateTime.Now;

            await _repository.UpdateAsync(notification);

            return new BaseResponseDTO
            {
                Success = true,
                Message = "Notification marked as read"
            };
        }

        public async Task<BaseResponseDTO> DeleteAsync(int id)
        {
            var notification = await _repository.GetByIdAsync(id);

            if (notification == null)
                return new BaseResponseDTO { Success = false, Message = "Notification not found" };

            await _repository.DeleteAsync(notification);

            return new BaseResponseDTO
            {
                Success = true,
                Message = "Notification deleted"
            };
        }

        private NotificationDTO Map(KmsNotification n)
        {
            return new NotificationDTO
            {
                NotificationId = n.NotificationId,
                UserId = n.UserId,
                StudentId = n.StudentId,
                Type = n.Type,
                Title = n.Title,
                Message = n.Message,
                RelatedEntityType = n.RelatedEntityType,
                RelatedEntityId = n.RelatedEntityId,
                IsRead = n.IsRead ?? false,
                ReadAt = n.ReadAt,
                CreatedAt = n.CreatedAt
            };
        }
    }
}
