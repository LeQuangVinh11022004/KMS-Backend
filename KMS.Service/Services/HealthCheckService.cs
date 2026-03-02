using KMS.Repository.Entities;
using KMS.Repository.Interfaces;
using KMS.Service.DTOs.HealthCheck;
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

        public HealthCheckService(IHealthCheckRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<HealthCheckDTO>> GetAllAsync()
        {
            var data = await _repo.GetAllAsync();

            return data.Select(x => new HealthCheckDTO
            {
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
                CheckedBy = userId // 🔥 Lấy từ JWT
            };

            await _repo.AddAsync(entity);
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
            entity.CheckedBy = userId; // update người sửa

            await _repo.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                throw new Exception("Health check not found");

            await _repo.DeleteAsync(entity);
        }
    }
}
