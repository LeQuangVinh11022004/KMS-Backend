using KMS.Service.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.Interfaces
{
    public interface ITeacherService
    {
        Task<BaseResponseDTO> GetAllTeachersAsync();
        Task<BaseResponseDTO> GetTeacherByIdAsync(int teacherId);
        Task<BaseResponseDTO> CreateTeacherAsync(CreateTeacherDTO dto);
        Task<BaseResponseDTO> UpdateTeacherAsync(int teacherId, UpdateTeacherDTO dto);
        Task<BaseResponseDTO> DeleteTeacherAsync(int teacherId);
        Task<BaseResponseDTO> SearchTeachersAsync(string keyword);
        Task<BaseResponseDTO> GetActiveTeachersAsync();
    }
}
