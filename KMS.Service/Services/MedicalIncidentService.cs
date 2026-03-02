using KMS.Repository.Entities;
using KMS.Repository.Interfaces;
using KMS.Service.DTOs.MedicalIncident;
using KMS.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.Services
{
    public class MedicalIncidentService : IMedicalIncidentService
    {
        private readonly IMedicalIncidentRepository _repository;

        public MedicalIncidentService(IMedicalIncidentRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<MedicalIncidentDTO>> GetAllAsync()
        {
            var data = await _repository.GetAllAsync();

            return data.Select(x => new MedicalIncidentDTO
            {
                StudentId = x.StudentId,
                IncidentDate = x.IncidentDate,
                Type = x.Type,
                Description = x.Description,
                ActionTaken = x.ActionTaken,
                Status = x.Status
            }).ToList();
        }

        public async Task<MedicalIncidentDTO?> GetByIdAsync(int id)
        {
            var x = await _repository.GetByIdAsync(id);
            if (x == null) return null;

            return new MedicalIncidentDTO
            {
                StudentId = x.StudentId,
                IncidentDate = x.IncidentDate,
                Type = x.Type,
                Description = x.Description,
                ActionTaken = x.ActionTaken,
                Status = x.Status
            };
        }

        public async Task<List<MedicalIncidentDTO>> GetByStudentIdAsync(int studentId)
        {
            var data = await _repository.GetByStudentIdAsync(studentId);

            return data.Select(x => new MedicalIncidentDTO
            {
                StudentId = x.StudentId,
                IncidentDate = x.IncidentDate,
                Type = x.Type,
                Description = x.Description,
                ActionTaken = x.ActionTaken,
                Status = x.Status
            }).ToList();
        }

        public async Task CreateAsync(MedicalIncidentDTO dto, int userId)
        {
            var entity = new KmsMedicalIncident
            {
                StudentId = dto.StudentId,
                IncidentDate = dto.IncidentDate ?? DateTime.Now,
                Type = dto.Type,
                Description = dto.Description,
                ActionTaken = dto.ActionTaken,
                Status = dto.Status,
                ReportedBy = userId
            };

            await _repository.AddAsync(entity);
        }

        public async Task UpdateAsync(int id, MedicalIncidentDTO dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                throw new Exception("Medical incident not found");

            entity.Type = dto.Type;
            entity.Description = dto.Description;
            entity.ActionTaken = dto.ActionTaken;
            entity.Status = dto.Status;
            entity.IncidentDate = dto.IncidentDate;

            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                throw new Exception("Medical incident not found");

            await _repository.DeleteAsync(entity);
        }
    }
}
