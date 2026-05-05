using KMS.Service.DTOs.ClassTeacher;
using KMS.Service.DTOs.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.Interfaces
{
    public interface IClassTeacherService
    {
        Task<BaseResponseDTO> GetTeachersByClassAsync(int classId);
        Task<BaseResponseDTO> GetClassesByTeacherAsync(int teacherId);
        Task<BaseResponseDTO> AssignTeacherAsync(int classId, AssignTeacherDTO dto, int assignedBy);
        Task<BaseResponseDTO> RemoveTeacherFromClassAsync(int classId, int teacherId);
    }
}
