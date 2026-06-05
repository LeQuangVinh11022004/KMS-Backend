using KMS.Repository.Entities;
using KMS.Repository.Interfaces;
using KMS.Service.DTOs.ClassActivity;
using KMS.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.Services
{
    public class ClassActivityService : IClassActivityService
    {
        private readonly IClassActivityRepository _repo;
        private readonly IActivityMediaRepository _mediaRepo;
        private readonly Supabase.Client _supabaseClient;

        public ClassActivityService(
            IClassActivityRepository repo,
            IActivityMediaRepository mediaRepo,
            Supabase.Client supabaseClient)
        {
            _repo = repo;
            _mediaRepo = mediaRepo;
            _supabaseClient = supabaseClient;
        }

        public async Task<List<ClassActivityDTO>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();

            return list.Select(x => new ClassActivityDTO
            {
                ActivityId = x.ActivityId,
                ClassId = x.ClassId,
                Title = x.Title,
                Content = x.Content,
                ActivityDate = x.ActivityDate
            }).ToList();
        }

        public async Task<List<ClassActivityDTO>> GetByClassIdAsync(int classId)
        {
            var list = await _repo.GetByClassIdAsync(classId);

            return list.Select(x => new ClassActivityDTO
            {
                ActivityId = x.ActivityId,
                ClassId = x.ClassId,
                Title = x.Title,
                Content = x.Content,
                ActivityDate = x.ActivityDate
            }).ToList();
        }

        public async Task<ClassActivityDTO?> GetByIdAsync(int id)
        {
            var x = await _repo.GetByIdAsync(id);
            if (x == null) return null;

            return new ClassActivityDTO
            {
                ActivityId = x.ActivityId,
                ClassId = x.ClassId,
                Title = x.Title,
                Content = x.Content,
                ActivityDate = x.ActivityDate
            };
        }

        public async Task CreateAsync(ClassActivityDTO dto, int teacherId)
        {
            var entity = new KmsClassActivity
            {
                ClassId = dto.ClassId,
                Title = dto.Title,
                Content = dto.Content,
                ActivityDate = dto.ActivityDate,
                PostedBy = teacherId,
                CreatedAt = DateTime.Now
            };

            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();
        }
        public async Task<int> CreateWithMediaAsync(ActivityWithMediaCreateDTO dto, List<IFormFile> passedFiles, int teacherId)
        {
            var activityEntity = new KmsClassActivity
            {
                ClassId = dto.ClassId,
                Title = dto.Title,
                Content = dto.Content,
                ActivityDate = dto.ActivityDate,
                PostedBy = teacherId,
                CreatedAt = DateTime.Now
            };

            await _repo.AddAsync(activityEntity);
            await _repo.SaveChangesAsync();

            int newActivityId = activityEntity.ActivityId;

            if (passedFiles != null && passedFiles.Any())
            {
                string bucketName = "activities";

                foreach (var file in passedFiles)
                {
                    if (file.Length == 0) continue;

                    string fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                    byte[] fileData;

                    using (var stream = file.OpenReadStream())
                    using (var ms = new MemoryStream())
                    {
                        await stream.CopyToAsync(ms);
                        fileData = ms.ToArray();
                    }

                    var options = new Supabase.Storage.FileOptions { ContentType = file.ContentType };
                    await _supabaseClient.Storage.From(bucketName).Upload(fileData, fileName, options);

                    string publicUrl = _supabaseClient.Storage.From(bucketName).GetPublicUrl(fileName);

                    var mediaEntity = new KmsActivityMedium
                    {
                        ActivityId = newActivityId,
                        MediaUrl = publicUrl,
                        MediaType = file.ContentType.StartsWith("video") ? "Video" : "Photo",
                        Caption = null,
                        UploadedAt = DateTime.Now
                    };

                    await _mediaRepo.AddAsync(mediaEntity);
                }

                await _mediaRepo.SaveChangesAsync();
            }

            return newActivityId;
        }

        public async Task UpdateAsync(int id, ClassActivityDTO dto, int teacherId)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                throw new Exception("Activity not found");

            entity.Title = dto.Title;
            entity.Content = dto.Content;
            entity.ActivityDate = dto.ActivityDate;
            entity.UpdatedAt = DateTime.Now;
            entity.UpdatedBy = teacherId;

            _repo.Update(entity);
            await _repo.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                throw new Exception("Activity not found");

            _repo.Delete(entity);
            await _repo.SaveChangesAsync();
        }
    }
}
