using KMS.Repository.Entities;
using KMS.Repository.Interfaces;
using KMS.Service.DTOs.ActivityPhoto;
using KMS.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.Services
{
    public class ActivityPhotoService : IActivityPhotoService
    {
        private readonly IActivityPhotoRepository _repo;
        private readonly IClassActivityRepository _activityRepo;

        public ActivityPhotoService(
            IActivityPhotoRepository repo,
            IClassActivityRepository activityRepo)
        {
            _repo = repo;
            _activityRepo = activityRepo;
        }

        public async Task<List<ActivityPhotoDTO>> GetByActivityIdAsync(int activityId)
        {
            var list = await _repo.GetByActivityIdAsync(activityId);

            return list.Select(x => new ActivityPhotoDTO
            {
                PhotoId = x.PhotoId,
                ActivityId = x.ActivityId,
                PhotoUrl = x.PhotoUrl,
                Caption = x.Caption
            }).ToList();
        }

        public async Task AddAsync(int activityId, ActivityPhotoDTO dto)
        {
            var activity = await _activityRepo.GetByIdAsync(activityId);
            if (activity == null)
                throw new Exception("Activity not found");

            var entity = new KmsActivityPhoto
            {
                ActivityId = activityId,
                PhotoUrl = dto.PhotoUrl,
                Caption = dto.Caption,
                UploadedAt = DateTime.Now
            };

            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();
        }

        public async Task DeleteAsync(int photoId)
        {
            var entity = await _repo.GetByIdAsync(photoId);
            if (entity == null)
                throw new Exception("Photo not found");

            _repo.Delete(entity);
            await _repo.SaveChangesAsync();
        }
    }
}
