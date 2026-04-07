using KMS.Repository.Entities;
using KMS.Repository.Interfaces;
using KMS.Service.DTOs.DailyReport;
using KMS.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.Services
{
    public class DailyReportService : IDailyReportService
    {
        private readonly IDailyReportRepository _repo;

        public DailyReportService(IDailyReportRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<DailyReportDTO>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();

            return list.Select(x => new DailyReportDTO
            {
                ReportId = x.ReportId,
                StudentId = x.StudentId,
                ClassId = x.ClassId,
                ReportDate = x.ReportDate,
                LivingData = x.LivingData,
                EducationalData = x.EducationalData,
                Status = x.Status,
                CreatedAt = x.CreatedAt
            }).ToList();
        }

        public async Task<DailyReportDTO?> GetByIdAsync(int id)
        {
            var x = await _repo.GetByIdAsync(id);

            if (x == null) return null;

            return new DailyReportDTO
            {
                ReportId = x.ReportId,
                StudentId = x.StudentId,
                ClassId = x.ClassId,
                ReportDate = x.ReportDate,
                LivingData = x.LivingData,
                EducationalData = x.EducationalData,
                Status = x.Status,
                CreatedAt = x.CreatedAt
            };
        }

        public async Task CreateAsync(CreateDailyReportDTO dto, int userId)
        {
            var entity = new KmsDailyReport
            {
                StudentId = dto.StudentId,
                ClassId = dto.ClassId,
                LivingData = dto.LivingData,
                EducationalData = dto.EducationalData,
                CreatedBy = userId,
                ReviewedBy = dto.ReviewedBy,
                Status = dto.Status ?? "Draft",
                CreatedAt = DateTime.Now,
                ReportDate = DateOnly.FromDateTime(DateTime.Today)
            };

            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();
        }

        public async Task UpdateAsync(int id, UpdateDailyReportDTO dto)
        {
            var entity = await _repo.GetByIdAsync(id);

            if (entity == null)
                throw new Exception("Daily report not found");

            entity.LivingData = dto.LivingData;
            entity.EducationalData = dto.EducationalData;
            entity.ReviewedBy = dto.ReviewedBy;
            entity.Status = dto.Status;
            entity.UpdatedAt = DateTime.Now;

            _repo.Update(entity);

            await _repo.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);

            if (entity == null)
                throw new Exception("Daily report not found");

            _repo.Delete(entity);

            await _repo.SaveChangesAsync();
        }
    }
}
