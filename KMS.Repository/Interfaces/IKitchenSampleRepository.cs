using KMS.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Repository.Interfaces
{
    public interface IKitchenSampleRepository
    {
        Task<List<KmsKitchenSample>> GetAllAsync();

        Task<KmsKitchenSample?> GetByIdAsync(int id);

        Task AddAsync(KmsKitchenSample entity);

        Task UpdateAsync(KmsKitchenSample entity);

        Task DeleteAsync(KmsKitchenSample entity);
    }
}
