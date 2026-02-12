using KMS.Repository.Entities;
using KMS.Repository.Interfaces;
using KMS.Service.DTOs.Semester;
using KMS.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.Services
{
    public class SemesterService : ISemesterService
    {
        private readonly ISemesterRepository _repo;

        public SemesterService(ISemesterRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<SemesterDTO>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();

            return list.Select(x => new SemesterDTO
            {
                SemesterId = x.SemesterId,
                SchoolYearId = x.SchoolYearId,
                SemesterName = x.SemesterName,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                IsActive = x.IsActive
            }).ToList();
        }

        public async Task<SemesterDTO?> GetByIdAsync(int id)
        {
            var x = await _repo.GetByIdAsync(id);
            if (x == null) return null;

            return new SemesterDTO
            {
                SemesterId = x.SemesterId,
                SchoolYearId = x.SchoolYearId,
                SemesterName = x.SemesterName,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                IsActive = x.IsActive
            };
        }

        public async Task CreateAsync(SemesterDTO dto)
        {
            if (dto.StartDate >= dto.EndDate)
                throw new Exception("StartDate must be before EndDate");

            var entity = new KmsSemester
            {
                SchoolYearId = dto.SchoolYearId,
                SemesterName = dto.SemesterName,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                IsActive = dto.IsActive
            };

            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();
        }

        public async Task UpdateAsync(int id, SemesterDTO dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                throw new Exception("Semester not found");

            entity.SemesterName = dto.SemesterName;
            entity.StartDate = dto.StartDate;
            entity.EndDate = dto.EndDate;
            entity.IsActive = dto.IsActive;

            _repo.Update(entity);
            await _repo.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                throw new Exception("Semester not found");

            _repo.Delete(entity);
            await _repo.SaveChangesAsync();
        }
    }
}
