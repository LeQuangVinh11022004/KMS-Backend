using KMS.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Repository.Interfaces
{
    public interface IParentRegistrationRepository
    {
        // Read
        Task<KmsParentRegistration?> GetByIdAsync(int registrationId);
        Task<IEnumerable<KmsParentRegistration>> GetAllAsync();
        Task<IEnumerable<KmsParentRegistration>> GetByStatusAsync(string status); // Pending, Approved, Rejected
        Task<bool> EmailAlreadyRegisteredAsync(string email);
        Task<bool> EmailAlreadyPendingAsync(string email);

        // Create
        Task<KmsParentRegistration> CreateAsync(KmsParentRegistration registration);

        // Update
        Task UpdateAsync(KmsParentRegistration registration);

        // Stats
        Task<int> CountByStatusAsync(string status);
    }
}
