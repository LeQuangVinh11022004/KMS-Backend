using KMS.Repository.Entities;
using KMS.Repository.Interfaces;
using KMS.Service.DTOs.Class;
using KMS.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.Services
{
    public class ClassService : IClassService
    {
        private readonly IClassRepository _repository;

        public ClassService(IClassRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ClassResponseDTO>> GetAllAsync()
        {
            var classes = await _repository.GetAllAsync();

            return classes.Select(c => new ClassResponseDTO
            {
                ClassId = c.ClassId,
                ClassName = c.ClassName,
                SchoolYearId = c.SchoolYearId,
                AgeGroup = c.AgeGroup,
                MaxCapacity = c.MaxCapacity,
                CurrentEnrollment = c.CurrentEnrollment,
                Room = c.Room,
                IsActive = c.IsActive
            }).ToList();
        }

        public async Task<ClassResponseDTO?> GetByIdAsync(int id)
        {
            var c = await _repository.GetByIdAsync(id);
            if (c == null) return null;

            return new ClassResponseDTO
            {
                ClassId = c.ClassId,
                ClassName = c.ClassName,
                SchoolYearId = c.SchoolYearId,
                AgeGroup = c.AgeGroup,
                MaxCapacity = c.MaxCapacity,
                CurrentEnrollment = c.CurrentEnrollment,
                Room = c.Room,
                IsActive = c.IsActive
            };
        }

        public async Task<ClassResponseDTO> CreateAsync(ClassCreateRequestDTO request, int createdBy)
        {
            var entity = new KmsClass
            {
                ClassName = request.ClassName,
                SchoolYearId = request.SchoolYearId,
                AgeGroup = request.AgeGroup,
                MaxCapacity = request.MaxCapacity,
                Room = request.Room,
                CurrentEnrollment = 0,
                IsActive = true,
                CreatedAt = DateTime.Now,
                CreatedBy = createdBy
            };

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

            return await GetByIdAsync(entity.ClassId)!;
        }

        public async Task<bool> UpdateAsync(int id, ClassUpdateRequestDTO request)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;

            entity.ClassName = request.ClassName;
            entity.AgeGroup = request.AgeGroup;
            entity.MaxCapacity = request.MaxCapacity;
            entity.Room = request.Room;
            entity.IsActive = request.IsActive;

            _repository.Update(entity);
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;

            _repository.Delete(entity);
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}
