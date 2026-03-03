using KMS.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Repository.Interfaces
{
    public interface IMedicalIncidentRepository
    {
        Task<List<KmsMedicalIncident>> GetAllAsync();
        Task<KmsMedicalIncident?> GetByIdAsync(int id);
        Task<List<KmsMedicalIncident>> GetByStudentIdAsync(int studentId);
        Task AddAsync(KmsMedicalIncident entity);
        Task UpdateAsync(KmsMedicalIncident entity);
        Task DeleteAsync(KmsMedicalIncident entity);
    }
}
