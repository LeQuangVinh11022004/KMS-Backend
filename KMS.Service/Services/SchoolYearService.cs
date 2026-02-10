using KMS.Repository.Entities;
using KMS.Repository.Interfaces;
using KMS.Service.DTOs.SchoolYear;
using KMS.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.Services
{
    public class SchoolYearService : ISchoolYearService
    {
        private readonly ISchoolYearRepository _repo;
        private readonly IUnitOfWork _unitOfWork;

        public SchoolYearService(
            ISchoolYearRepository repo,
            IUnitOfWork unitOfWork)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<SchoolYearDTO>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(x => new SchoolYearDTO
            {
                SchoolYearId = x.SchoolYearId,
                YearName = x.YearName,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                IsActive = x.IsActive
            }).ToList();
        }

        public async Task<SchoolYearDTO?> GetByIdAsync(int id)
        {
            var x = await _repo.GetByIdAsync(id);
            if (x == null) return null;

            return new SchoolYearDTO
            {
                SchoolYearId = x.SchoolYearId,
                YearName = x.YearName,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                IsActive = x.IsActive
            };
        }

        public async Task CreateAsync(SchoolYearCreateDTO dto, int createdBy)
        {
            var entity = new KmsSchoolYear
            {
                YearName = dto.YearName,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                IsActive = true,
                CreatedAt = DateTime.Now,
                CreatedBy = createdBy
            };

            await _repo.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(int id, SchoolYearUpdateDTO dto, int updatedBy)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return false;

            entity.YearName = dto.YearName;
            entity.StartDate = dto.StartDate;
            entity.EndDate = dto.EndDate;
            entity.IsActive = dto.IsActive;

            _repo.Update(entity);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
