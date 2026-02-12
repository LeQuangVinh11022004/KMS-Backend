using KMS.Service.DTOs.Campus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.Interfaces
{
    public interface ICampusService
    {
        Task<List<CampusDTO>> GetAllAsync();
        Task<CampusDTO?> GetByIdAsync(int id);
        Task CreateAsync(CampusDTO dto);
        Task UpdateAsync(int id, CampusDTO dto);
        Task DeleteAsync(int id);
    }
}
