using KMS.Repository.Entities;
using KMS.Repository.Interfaces;
using KMS.Service.DTOs.ClassActivity;
using KMS.Service.Interfaces;
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

        public ClassActivityService(IClassActivityRepository repo)
        {
            _repo = repo;
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
