using KMS.Repository.Entities;
using KMS.Repository.Interfaces;
using KMS.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KMS.Service.DTOs.ActivityMedia;

namespace KMS.Service.Services
{
    public class ActivityMediaService : IActivityMediaService
    {
        private readonly IActivityMediaRepository _repo;
        private readonly IClassActivityRepository _activityRepo;

        public ActivityMediaService(
            IActivityMediaRepository repo,
            IClassActivityRepository activityRepo)
        {
            _repo = repo;
            _activityRepo = activityRepo;
        }

        public async Task<List<ActivityMediaDTO>> GetByActivityIdAsync(int activityId)
        {
            var list = await _repo.GetByActivityIdAsync(activityId);

            return list.Select(x => new ActivityMediaDTO
            {
                MediaId = x.MediaId,
                ActivityId = x.ActivityId,
                MediaUrl = x.MediaUrl,
                MediaType = x.MediaType,
                Caption = x.Caption,
                UploadedAt = x.UploadedAt ?? DateTime.Now
            }).ToList();
        }

        public async Task AddAsync(int activityId, ActivityMediaDTO dto)
        {
            var activity = await _activityRepo.GetByIdAsync(activityId);

            if (activity == null)
                throw new Exception("Activity not found");

            var entity = new KmsActivityMedium
            {
                ActivityId = activityId,
                MediaUrl = dto.MediaUrl,
                MediaType = dto.MediaType,
                Caption = dto.Caption,
                UploadedAt = DateTime.Now
            };

            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();
        }

        public async Task DeleteAsync(int mediaId)
        {
            var entity = await _repo.GetByIdAsync(mediaId);

            if (entity == null)
                throw new Exception("Media not found");

            _repo.Delete(entity);
            await _repo.SaveChangesAsync();
        }
    }
}