using KMS.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Repository.Interfaces
{
    public interface ICampusRepository
    {
        Task<List<KmsCampus>> GetAllAsync();
        Task<KmsCampus?> GetByIdAsync(int id);
        Task AddAsync(KmsCampus entity);
        void Update(KmsCampus entity);
        Task SaveChangesAsync();
    }
}
