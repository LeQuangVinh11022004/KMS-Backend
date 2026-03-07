using KMS.Service.DTOs.KitchenSample;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.Interfaces
{
    public interface IKitchenSampleService
    {
        Task<List<KitchenSampleDTO>> GetAllAsync();

        Task<KitchenSampleDTO?> GetByIdAsync(int id);

        Task CreateAsync(KitchenSampleDTO dto, int userId);

        Task UpdateAsync(int id, KitchenSampleDTO dto);

        Task DeleteAsync(int id);
    }
}
