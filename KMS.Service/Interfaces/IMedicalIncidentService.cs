using KMS.Repository.Entities;
using KMS.Service.DTOs.MedicalIncident;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.Interfaces
{
    public interface IMedicalIncidentService
    {
        Task<List<MedicalIncidentDTO>> GetAllAsync();
        Task<MedicalIncidentDTO?> GetByIdAsync(int id);
        Task<List<MedicalIncidentDTO>> GetByStudentIdAsync(int studentId);
        Task CreateAsync(MedicalIncidentDTO dto, int userId);
        Task UpdateAsync(int id, MedicalIncidentDTO dto);
        Task DeleteAsync(int id);
    }
}
