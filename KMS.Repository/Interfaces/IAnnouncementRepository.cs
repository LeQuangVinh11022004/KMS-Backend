using KMS.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Repository.Interfaces
{
    public interface IAnnouncementRepository
    {
        Task<List<KmsAnnouncement>> GetAllAsync();
        Task<KmsAnnouncement?> GetByIdAsync(int id);
        Task AddAsync(KmsAnnouncement entity);
        void Update(KmsAnnouncement entity);
        void Delete(KmsAnnouncement entity);
        Task SaveChangesAsync();
    }
}
