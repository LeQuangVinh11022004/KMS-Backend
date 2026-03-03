using KMS.Repository.Entities;
using KMS.Service.DTOs.HealthCheck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.Interfaces
{
    public interface IHealthCheckService
    {
        Task<List<HealthCheckDTO>> GetAllAsync();
        Task<HealthCheckDTO?> GetByIdAsync(int id);
        Task<List<HealthCheckDTO>> GetByStudentIdAsync(int studentId);
        Task CreateAsync(HealthCheckDTO dto, int userId);
        Task UpdateAsync(int id, HealthCheckDTO dto, int userId);
        Task DeleteAsync(int id);
    }
}
