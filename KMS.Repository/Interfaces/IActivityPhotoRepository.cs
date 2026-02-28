using KMS.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Repository.Interfaces
{
    public interface IActivityPhotoRepository
    {
        Task<List<KmsActivityPhoto>> GetByActivityIdAsync(int activityId);
        Task<KmsActivityPhoto?> GetByIdAsync(int photoId);
        Task AddAsync(KmsActivityPhoto entity);
        void Delete(KmsActivityPhoto entity);
        Task SaveChangesAsync();
    }
}
