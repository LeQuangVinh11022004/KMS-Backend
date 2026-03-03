using KMS.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Repository.Interfaces
{
    public interface IHealthCheckRepository
    {
        Task<List<KmsHealthCheck>> GetAllAsync();
        Task<KmsHealthCheck?> GetByIdAsync(int id);
        Task<List<KmsHealthCheck>> GetByStudentIdAsync(int studentId);
        Task AddAsync(KmsHealthCheck entity);
        Task UpdateAsync(KmsHealthCheck entity);
        Task DeleteAsync(KmsHealthCheck entity);
    }
}
