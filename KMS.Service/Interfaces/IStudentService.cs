using KMS.Service.DTOs.Role;
using KMS.Service.DTOs.Student;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.Interfaces
{
    public interface IStudentService
    {
        Task<BaseResponseDTO> GetAllStudentsAsync();
        Task<BaseResponseDTO> GetStudentByIdAsync(int studentId);
        Task<BaseResponseDTO> CreateStudentAsync(CreateStudentDTO dto);
        Task<BaseResponseDTO> UpdateStudentAsync(int studentId, UpdateStudentDTO dto);
        Task<BaseResponseDTO> DeleteStudentAsync(int studentId);
        Task<BaseResponseDTO> SearchStudentsAsync(string keyword);
        Task<BaseResponseDTO> GetActiveStudentsAsync();
        Task<BaseResponseDTO> GetStudentsByParentIdAsync(int parentId);
    }
}
