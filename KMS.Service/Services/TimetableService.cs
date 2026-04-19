using KMS.Repository.Entities;
using KMS.Repository.Interfaces;
using KMS.Service.DTOs.Role;
using KMS.Service.DTOs.Timetable;
using KMS.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.Services
{
    public class TimetableService : ITimetableService
    {
        private readonly ITimetableRepository _repository;

        public TimetableService(ITimetableRepository repository)
        {
            _repository = repository;
        }

        public async Task<BaseResponseDTO> GetByClassIdAsync(int classId)
        {
            var data = await _repository.GetByClassIdAsync(classId);
            var mappedData = data.Select(Map).ToList();
            return new BaseResponseDTO { Success = true, Message = "Success", Data = mappedData };
        }

        public async Task<BaseResponseDTO> GetByTeacherIdAsync(int teacherId)
        {
            var data = await _repository.GetByTeacherIdAsync(teacherId);
            var mappedData = data.Select(Map).ToList();
            return new BaseResponseDTO { Success = true, Message = "Success", Data = mappedData };
        }

        public async Task<BaseResponseDTO> CreateAsync(CreateTimetableDTO dto, int createdBy)
        {
            var entity = new KmsTimetable
            {
                ClassId = dto.ClassId,
                DayOfWeek = dto.DayOfWeek,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Subject = dto.Subject,
                TeacherId = dto.TeacherId,
                Room = dto.Room,
                IsActive = true,
                CreatedAt = DateTime.Now,
                CreatedBy = createdBy
            };

            await _repository.CreateAsync(entity);

            return new BaseResponseDTO { Success = true, Message = "Timetable created successfully", Data = Map(entity) };
        }

        public async Task<BaseResponseDTO> UpdateAsync(int id, UpdateTimetableDTO dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return new BaseResponseDTO { Success = false, Message = "Timetable not found" };

            entity.DayOfWeek = dto.DayOfWeek;
            entity.StartTime = dto.StartTime;
            entity.EndTime = dto.EndTime;
            entity.Subject = dto.Subject;
            entity.TeacherId = dto.TeacherId;
            entity.Room = dto.Room;
            entity.IsActive = dto.IsActive;

            await _repository.UpdateAsync(entity);

            return new BaseResponseDTO { Success = true, Message = "Timetable updated successfully" };
        }

        public async Task<BaseResponseDTO> DeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return new BaseResponseDTO { Success = false, Message = "Timetable not found" };

            await _repository.DeleteAsync(entity);

            return new BaseResponseDTO { Success = true, Message = "Timetable deleted successfully" };
        }

        private TimetableDTO Map(KmsTimetable t) => new TimetableDTO
        {
            TimetableId = t.TimetableId,
            ClassId = t.ClassId,
            DayOfWeek = t.DayOfWeek,
            StartTime = t.StartTime,
            EndTime = t.EndTime,
            Subject = t.Subject,
            TeacherId = t.TeacherId,
            Room = t.Room,
            IsActive = t.IsActive ?? false
        };
    }
}
