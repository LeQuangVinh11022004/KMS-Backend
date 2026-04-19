using KMS.Service.DTOs.Role;
using KMS.Service.DTOs.Timetable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.Interfaces
{
    public interface ITimetableService
    {
        Task<BaseResponseDTO> GetByClassIdAsync(int classId);
        Task<BaseResponseDTO> GetByTeacherIdAsync(int teacherId);
        Task<BaseResponseDTO> CreateAsync(CreateTimetableDTO dto, int createdBy);
        Task<BaseResponseDTO> UpdateAsync(int id, UpdateTimetableDTO dto);
        Task<BaseResponseDTO> DeleteAsync(int id);
    }
}
