using KMS.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Repository.Interfaces
{
    public interface IStudentRepository
    {
        // CRUD
        Task<KmsStudent?> GetByIdAsync(int studentId);
        Task<KmsStudent?> GetByStudentCodeAsync(string studentCode);
        Task<IEnumerable<KmsStudent>> GetAllAsync();
        Task<KmsStudent> CreateAsync(KmsStudent student);
        Task UpdateAsync(KmsStudent student);
        Task DeleteAsync(int studentId);

        // Filter
        Task<IEnumerable<KmsStudent>> GetActiveStudentsAsync();
        Task<IEnumerable<KmsStudent>> GetInactiveStudentsAsync();
        Task<IEnumerable<KmsStudent>> GetStudentsByClassIdAsync(int classId);
        Task<IEnumerable<KmsStudent>> GetStudentsByParentIdAsync(int parentId);

        // Search
        Task<IEnumerable<KmsStudent>> SearchStudentsAsync(string keyword);

        // Check
        Task<bool> StudentCodeExistsAsync(string studentCode);

        // Statistics
        Task<int> CountActiveStudentsAsync();
    }
}
