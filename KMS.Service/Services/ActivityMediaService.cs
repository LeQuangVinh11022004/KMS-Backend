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
        private readonly Supabase.Client _supabaseClient;
        public ActivityMediaService(
                    IActivityMediaRepository repo,
                    IClassActivityRepository activityRepo,
                    Supabase.Client supabaseClient)
        {
            _repo = repo;
            _activityRepo = activityRepo;
            _supabaseClient = supabaseClient;
        }

        public async Task<List<ActivityMediaDTO>> GetByActivityIdAsync(int activityId, string baseUrl)
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

        public async Task<ActivityMediaDTO> AddAsync(int activityId, ActivityMediaCreateDTO dto, string webRootPath, string baseUrl)
        {
            try
            {
                var activity = await _activityRepo.GetByIdAsync(activityId);
                if (activity == null)
                    throw new Exception("Activity not found");

                if (dto.File == null || dto.File.Length == 0)
                    throw new Exception("No file uploaded.");

                string bucketName = "activities"; // ten cua bucket tao tren supabase
                string fileName = $"{Guid.NewGuid()}_{Path.GetFileName(dto.File.FileName)}";

                byte[] fileData;
                using (var stream = dto.File.OpenReadStream())
                using (var ms = new MemoryStream())
                {
                    await stream.CopyToAsync(ms);
                    fileData = ms.ToArray();
                }

                var options = new Supabase.Storage.FileOptions { ContentType = dto.File.ContentType };
                await _supabaseClient.Storage.From(bucketName).Upload(fileData, fileName, options);

                string publicUrl = _supabaseClient.Storage.From(bucketName).GetPublicUrl(fileName);

                var entity = new KmsActivityMedium
                {
                    ActivityId = activityId,
                    MediaUrl = publicUrl,
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
                    MediaUrl = entity.MediaUrl,
                    MediaType = entity.MediaType,
                    Caption = entity.Caption,
                    UploadedAt = entity.UploadedAt.Value
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi thực tế khi xử lý Supabase Storage: {ex.Message} | Inner: {ex.InnerException?.Message}");
            }
        }

        public async Task DeleteAsync(int mediaId)
        {
            var entity = await _repo.GetByIdAsync(mediaId);
            if (entity == null)
                throw new Exception("Media not found");

            try
            {
                string bucketName = "activities";
                string fileName = Path.GetFileName(entity.MediaUrl);
                await _supabaseClient.Storage.From(bucketName).Remove(new List<string> { fileName });
            }
            catch {  }

            _repo.Delete(entity);
            await _repo.SaveChangesAsync();
        }
    }
}
