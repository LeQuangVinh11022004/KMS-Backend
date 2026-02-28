using KMS.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Repository.Interfaces
{
    public interface IClassActivityRepository
    {
        Task<List<KmsClassActivity>> GetAllAsync();
        Task<List<KmsClassActivity>> GetByClassIdAsync(int classId);
        Task<KmsClassActivity?> GetByIdAsync(int id);
        Task AddAsync(KmsClassActivity entity);
        void Update(KmsClassActivity entity);
        void Delete(KmsClassActivity entity);
        Task SaveChangesAsync();
    }
}
