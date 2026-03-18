using KMS.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Repository.Interfaces
{
    public interface IActivityMediaRepository
    {
        Task<List<KmsActivityMedium>> GetByActivityIdAsync(int activityId);
        Task<KmsActivityMedium?> GetByIdAsync(int mediaId);
        Task AddAsync(KmsActivityMedium entity);
        void Delete(KmsActivityMedium entity);
        Task SaveChangesAsync();
    }
}
