using KMS.Service.DTOs.SchoolYear;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.Interfaces
{
    public interface ISchoolYearService
    {
        Task<List<SchoolYearDTO>> GetAllAsync();
        Task<SchoolYearDTO?> GetByIdAsync(int id);
        Task CreateAsync(SchoolYearCreateDTO dto, int createdBy);
        Task<bool> UpdateAsync(int id, SchoolYearUpdateDTO dto, int updatedBy);
    }
}
