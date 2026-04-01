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

        public async Task<List<ActivityMediaDTO>> GetByActivityIdAsync(int activityId, string baseUrl)
        {
            var list = await _repo.GetByActivityIdAsync(activityId);

            return list.Select(x => new ActivityMediaDTO
            {
                MediaId = x.MediaId,
                ActivityId = x.ActivityId,
                MediaUrl = x.MediaUrl.StartsWith("http") ? x.MediaUrl : $"{baseUrl}/{x.MediaUrl}",
                MediaType = x.MediaType,
                Caption = x.Caption,
                UploadedAt = x.UploadedAt ?? DateTime.Now
            }).ToList();
        }

        public async Task<ActivityMediaDTO> AddAsync(int activityId, ActivityMediaCreateDTO dto, string webRootPath, string baseUrl)
        {
            var activity = await _activityRepo.GetByIdAsync(activityId);

            if (activity == null)
                throw new Exception("Activity not found");

            if (dto.File == null || dto.File.Length == 0)
                throw new Exception("No file uploaded.");

            var uploadFolder = Path.Combine(webRootPath, "uploads", "activities");
            if (!Directory.Exists(uploadFolder))
                Directory.CreateDirectory(uploadFolder);

            // create file unqiue name to avoid conflict
            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(dto.File.FileName)}";
            var filePath = Path.Combine(uploadFolder, fileName);

            // save file to server
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.File.CopyToAsync(stream);
            }

            var relativePath = $"uploads/activities/{fileName}";

            var entity = new KmsActivityMedium
            {
                ActivityId = activityId,
                MediaUrl = relativePath,
                MediaType = dto.MediaType,
                Caption = dto.Caption,
                UploadedAt = DateTime.Now
            };

            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();

            return new ActivityMediaDTO
            {
                MediaId = entity.MediaId,
                ActivityId = entity.ActivityId,
                MediaUrl = $"{baseUrl}/{relativePath}",
                MediaType = entity.MediaType,
                Caption = entity.Caption,
                UploadedAt = entity.UploadedAt.Value
            };
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