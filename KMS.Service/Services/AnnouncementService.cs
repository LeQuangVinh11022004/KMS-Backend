using KMS.Repository.Entities;
using KMS.Repository.Interfaces;
using KMS.Service.DTOs.Announcement;
using KMS.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.Services
{
    public class AnnouncementService : IAnnouncementService
    {
        private readonly IAnnouncementRepository _repo;

        public AnnouncementService(IAnnouncementRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<AnnouncementDTO>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();

            return list.Select(x => new AnnouncementDTO
            {
                AnnouncementId = x.AnnouncementId,
                Title = x.Title,
                Content = x.Content,
                TargetAudience = x.TargetAudience,
                TargetClassId = x.TargetClassId,
                Priority = x.Priority,
                IsPublished = x.IsPublished,
                PublishedAt = x.PublishedAt,
                ExpiresAt = x.ExpiresAt
            }).ToList();
        }

        public async Task<AnnouncementDTO?> GetByIdAsync(int id)
        {
            var x = await _repo.GetByIdAsync(id);
            if (x == null) return null;

            return new AnnouncementDTO
            {
                AnnouncementId = x.AnnouncementId,
                Title = x.Title,
                Content = x.Content,
                TargetAudience = x.TargetAudience,
                TargetClassId = x.TargetClassId,
                Priority = x.Priority,
                IsPublished = x.IsPublished,
                PublishedAt = x.PublishedAt,
                ExpiresAt = x.ExpiresAt
            };
        }

        public async Task CreateAsync(AnnouncementDTO dto, int userId)
        {
            var entity = new KmsAnnouncement
            {
                Title = dto.Title,
                Content = dto.Content,
                TargetAudience = dto.TargetAudience,
                TargetClassId = dto.TargetClassId,
                Priority = dto.Priority,
                IsPublished = false,
                CreatedBy = userId,
                CreatedAt = DateTime.Now
            };

            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();
        }

        public async Task UpdateAsync(int id, AnnouncementDTO dto, int userId)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) throw new Exception("Announcement not found");

            entity.Title = dto.Title;
            entity.Content = dto.Content;
            entity.TargetAudience = dto.TargetAudience;
            entity.TargetClassId = dto.TargetClassId;
            entity.Priority = dto.Priority;
            entity.UpdatedBy = userId;
            entity.UpdatedAt = DateTime.Now;

            _repo.Update(entity);
            await _repo.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) throw new Exception("Announcement not found");

            _repo.Delete(entity);
            await _repo.SaveChangesAsync();
        }
    }
}
