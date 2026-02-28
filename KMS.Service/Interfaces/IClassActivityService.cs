using KMS.Service.DTOs.ClassActivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.Interfaces
{
    public interface IClassActivityService
    {
        Task<List<ClassActivityDTO>> GetAllAsync();
        Task<List<ClassActivityDTO>> GetByClassIdAsync(int classId);
        Task<ClassActivityDTO?> GetByIdAsync(int id);
        Task CreateAsync(ClassActivityDTO dto, int teacherId);
        Task UpdateAsync(int id, ClassActivityDTO dto, int teacherId);
        Task DeleteAsync(int id);
    }
}
