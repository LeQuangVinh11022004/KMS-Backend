using KMS.Service.DTOs.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.Interfaces
{
    public interface IClassService
    {
        Task<List<ClassResponseDTO>> GetAllAsync();
        Task<ClassResponseDTO?> GetByIdAsync(int id);
        Task<ClassResponseDTO> CreateAsync(ClassCreateRequestDTO request, int createdBy);
        Task<bool> UpdateAsync(int id, ClassUpdateRequestDTO request);
        Task<bool> DeleteAsync(int id);
    }
}
