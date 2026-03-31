using KMS.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Repository.Interfaces
{
    public interface INotificationRepository
    {
        Task<IEnumerable<KmsNotification>> GetByUserIdAsync(int userId);

        Task<KmsNotification?> GetByIdAsync(int id);

        Task CreateAsync(KmsNotification notification);
        Task CreateBulkAsync(IEnumerable<KmsNotification> notifications);
        Task UpdateAsync(KmsNotification notification);

        Task DeleteAsync(KmsNotification notification);
        Task<IEnumerable<KmsNotification>> GetByStudentIdAsync(int studentId);
    }
}
