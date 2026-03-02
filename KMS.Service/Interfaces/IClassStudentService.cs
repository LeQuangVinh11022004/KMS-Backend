using KMS.Service.DTOs.ClassStudent;
using KMS.Service.DTOs.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.Interfaces
{
    public interface IClassStudentService
    {
        Task<BaseResponseDTO> GetStudentsByClassAsync(int classId);
        Task<BaseResponseDTO> GetClassesByStudentAsync(int studentId);
        Task<BaseResponseDTO> EnrollStudentAsync(int classId, EnrollStudentDTO dto, int enrolledBy);
        Task<BaseResponseDTO> EnrollMultipleStudentsAsync(int classId, EnrollMultipleStudentsDTO dto, int enrolledBy);
        Task<BaseResponseDTO> RemoveStudentFromClassAsync(int classId, int studentId);
        Task<BaseResponseDTO> TransferStudentAsync(int studentId, int fromClassId, int toClassId, int enrolledBy);
    }
}
