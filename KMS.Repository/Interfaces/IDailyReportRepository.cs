using KMS.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Repository.Interfaces
{
    public interface IDailyReportRepository
    {
        Task<List<KmsDailyReport>> GetAllAsync();

        Task<KmsDailyReport?> GetByIdAsync(int reportId);

        Task AddAsync(KmsDailyReport entity);

        void Update(KmsDailyReport entity);

        void Delete(KmsDailyReport entity);

        Task SaveChangesAsync();
    }
}
