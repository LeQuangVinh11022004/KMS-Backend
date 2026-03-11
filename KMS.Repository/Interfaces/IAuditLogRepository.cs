using KMS.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Repository.Interfaces
{
    public interface IAuditLogRepository
    {
        Task<IEnumerable<KmsAuditLog>> GetAllAsync();
        Task<IEnumerable<KmsAuditLog>> GetFilteredAsync(
            int? userId, string? action, string? tableName,
            DateTime? fromDate, DateTime? toDate);
        Task<KmsAuditLog> CreateAsync(KmsAuditLog log);
    }
}
