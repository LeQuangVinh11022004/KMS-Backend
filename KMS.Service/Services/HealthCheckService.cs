using KMS.Repository.Entities;
using KMS.Repository.Interfaces;
using KMS.Service.DTOs.HealthCheck;
using KMS.Service.DTOs.Notification;
using KMS.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.Services
{
    public class HealthCheckService : IHealthCheckService
    {
        private readonly IHealthCheckRepository _repo;
        private readonly INotificationRepository _notificationRepo;
        private readonly IParentRepository _parentRepo;
        private readonly INotificationHubService _notificationHub;
        public HealthCheckService(
            IHealthCheckRepository repo,
            INotificationRepository notificationRepo,
            IParentRepository parentRepo,
            INotificationHubService notificationHub)
        {
            _repo = repo;
            _notificationRepo = notificationRepo;
            _parentRepo = parentRepo;
            _notificationHub = notificationHub;
        }

        public async Task<List<HealthCheckDTO>> GetAllAsync()
        {
            var data = await _repo.GetAllAsync();

            return data.Select(x => new HealthCheckDTO
            {
                CheckId = x.CheckId,
                StudentId = x.StudentId,
                CheckDate = x.CheckDate,
                Height = x.Height,
                Weight = x.Weight,
                EyeSight = x.EyeSight,
                DentalStatus = x.DentalStatus,
                Note = x.Note
            }).ToList();
        }

        public async Task<HealthCheckDTO?> GetByIdAsync(int id)
        {
            var x = await _repo.GetByIdAsync(id);
            if (x == null) return null;

            return new HealthCheckDTO
            {
                CheckId = x.CheckId,
                StudentId = x.StudentId,
                CheckDate = x.CheckDate,
                Height = x.Height,
                Weight = x.Weight,
                EyeSight = x.EyeSight,
                DentalStatus = x.DentalStatus,
                Note = x.Note
            };
        }

        public async Task<List<HealthCheckDTO>> GetByStudentIdAsync(int studentId)
        {
            var data = await _repo.GetByStudentIdAsync(studentId);

            return data.Select(x => new HealthCheckDTO
            {
                CheckId = x.CheckId,
                StudentId = x.StudentId,
                CheckDate = x.CheckDate,
                Height = x.Height,
                Weight = x.Weight,
                EyeSight = x.EyeSight,
                DentalStatus = x.DentalStatus,
                Note = x.Note
            }).ToList();
        }

        public async Task CreateAsync(HealthCheckDTO dto, int userId)
        {
            var entity = new KmsHealthCheck
            {
                StudentId = dto.StudentId,
                CheckDate = dto.CheckDate ?? DateOnly.FromDateTime(DateTime.Now),
                Height = dto.Height,
                Weight = dto.Weight,
                EyeSight = dto.EyeSight,
                DentalStatus = dto.DentalStatus,
                Note = dto.Note,
                CheckedBy = userId 
            };

            await _repo.AddAsync(entity);
            await SendHealthNotificationAsync(entity.StudentId, entity.CheckDate);
        }

        public async Task UpdateAsync(int id, HealthCheckDTO dto, int userId)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                throw new Exception("Health check not found");

            entity.CheckDate = dto.CheckDate;
            entity.Height = dto.Height;
            entity.Weight = dto.Weight;
            entity.EyeSight = dto.EyeSight;
            entity.DentalStatus = dto.DentalStatus;
            entity.Note = dto.Note;
            entity.CheckedBy = userId; 

            await _repo.UpdateAsync(entity);
            await SendHealthNotificationAsync(entity.StudentId, entity.CheckDate);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                throw new Exception("Health check not found");

            await _repo.DeleteAsync(entity);
        }
        private async Task SendHealthNotificationAsync(int studentId, DateOnly? checkDate)
        {
            var parents = await _parentRepo.GetParentsByStudentIdAsync(studentId);
            if (!parents.Any()) return;

            var notifications = parents.Select(p => new KmsNotification
            {
                UserId = p.UserId,
                StudentId = studentId,
                Type = "Health",
                Title = "Cập nhật khám sức khỏe",
                Message = $"Nhà trường vừa cập nhật chỉ số sức khỏe (chiều cao, cân nặng...) cho con bạn vào ngày {checkDate}.",
                RelatedEntityType = "HealthCheck",
                RelatedEntityId = studentId,
                IsRead = false,
                CreatedAt = DateTime.Now
            }).ToList();

            await _notificationRepo.CreateBulkAsync(notifications);

            foreach (var n in notifications)
            {
                await _notificationHub.SendNotificationToUserAsync(n.UserId, new NotificationDTO
                {
                    NotificationId = n.NotificationId,
                    UserId = n.UserId,
                    StudentId = n.StudentId,
                    Type = n.Type,
                    Title = n.Title,
                    Message = n.Message,
                    RelatedEntityType = n.RelatedEntityType,
                    RelatedEntityId = n.RelatedEntityId,
                    IsRead = false,
                    CreatedAt = n.CreatedAt
                });
            }
        }
    }
}
