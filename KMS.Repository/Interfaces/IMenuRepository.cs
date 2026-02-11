using KMS.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Repository.Interfaces
{
    public interface IMenuRepository
    {
        Task<List<KmsMenu>> GetAllAsync();
        Task<KmsMenu?> GetByIdAsync(int id);
        Task<List<KmsMenu>> GetByClassIdAsync(int classId);
        Task AddAsync(KmsMenu entity);
        void Update(KmsMenu entity);
        void Delete(KmsMenu entity);
    }
}
