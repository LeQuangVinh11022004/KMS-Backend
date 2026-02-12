using KMS.Service.DTOs.Semester;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.Interfaces
{
    public interface ISemesterService
    {
        Task<List<SemesterDTO>> GetAllAsync();
        Task<SemesterDTO?> GetByIdAsync(int id);
        Task CreateAsync(SemesterDTO dto);
        Task UpdateAsync(int id, SemesterDTO dto);
        Task DeleteAsync(int id);
    }
}
