using KMS.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Repository.Interfaces
{
    public interface IStudentParentRepository
    {
        // Read
        Task<IEnumerable<KmsStudentParent>> GetParentsByStudentAsync(int studentId);
        Task<IEnumerable<KmsStudentParent>> GetStudentsByParentAsync(int parentId);
        Task<KmsStudentParent?> GetAsync(int studentId, int parentId);
        Task<bool> LinkExistsAsync(int studentId, int parentId);
        Task<bool> HasPrimaryContactAsync(int studentId);

        // Create
        Task<KmsStudentParent> LinkAsync(KmsStudentParent studentParent);

        // Update
        Task UpdateAsync(KmsStudentParent studentParent);

        // Delete
        Task UnlinkAsync(int studentId, int parentId);
        Task UnlinkAllParentsFromStudentAsync(int studentId);
    }
}
