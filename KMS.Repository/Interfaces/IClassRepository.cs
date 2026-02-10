using KMS.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Repository.Interfaces
{
    public interface IClassRepository
    {
        Task<List<KmsClass>> GetAllAsync();
        Task<KmsClass?> GetByIdAsync(int id);
        Task AddAsync(KmsClass entity);
        void Update(KmsClass entity);
        void Delete(KmsClass entity);
        Task SaveChangesAsync();
    }
}
