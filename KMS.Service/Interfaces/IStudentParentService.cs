using KMS.Service.DTOs.Role;
using KMS.Service.DTOs.StudentParent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.Interfaces
{
    public interface IStudentParentService
    {
        Task<BaseResponseDTO> GetParentsByStudentAsync(int studentId);
        Task<BaseResponseDTO> GetStudentsByParentAsync(int parentId);
        Task<BaseResponseDTO> LinkAsync(int studentId, LinkStudentParentDTO dto);
        Task<BaseResponseDTO> UpdateLinkAsync(int studentId, int parentId, UpdateStudentParentDTO dto);
        Task<BaseResponseDTO> UnlinkAsync(int studentId, int parentId);
    }
}
