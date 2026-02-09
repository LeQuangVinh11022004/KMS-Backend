using KMS.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Repository.Interfaces
{
    public interface IParentRepository
    {
        // CRUD
        Task<KmsParent?> GetByIdAsync(int parentId);
        Task<KmsParent?> GetByUserIdAsync(int userId);
        Task<IEnumerable<KmsParent>> GetAllAsync();
        Task<KmsParent> CreateAsync(KmsParent parent);
        Task UpdateAsync(KmsParent parent);
        Task DeleteAsync(int parentId);

        // Get parents by student
        Task<IEnumerable<KmsParent>> GetParentsByStudentIdAsync(int studentId);

        // Search
        Task<IEnumerable<KmsParent>> SearchParentsAsync(string keyword);

        // Check
        Task<bool> UserIsParentAsync(int userId);
    }
}
