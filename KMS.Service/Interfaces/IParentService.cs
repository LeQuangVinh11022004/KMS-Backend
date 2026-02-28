using KMS.Service.DTOs.Parent;
using KMS.Service.DTOs.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.Interfaces
{
    public interface IParentService
    {
        Task<BaseResponseDTO> GetAllParentsAsync();
        Task<BaseResponseDTO> GetParentByIdAsync(int parentId);
        Task<BaseResponseDTO> CreateParentAsync(CreateParentDTO dto);
        Task<BaseResponseDTO> UpdateParentAsync(int parentId, UpdateParentDTO dto);
        Task<BaseResponseDTO> DeleteParentAsync(int parentId);
        Task<BaseResponseDTO> GetParentsByStudentIdAsync(int studentId);
        Task<BaseResponseDTO> SearchParentsAsync(string keyword);
    }
}
