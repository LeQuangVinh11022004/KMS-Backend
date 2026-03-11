using KMS.Repository.Entities;
using KMS.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Repository.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly ApplicationDbContext _context;

        public AuditLogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<KmsAuditLog>> GetAllAsync()
        {
            return await _context.KmsAuditLogs
                .Include(l => l.User)
                .OrderByDescending(l => l.CreatedAt)
                .Take(500) // Giới hạn 500 record gần nhất tránh quá tải
                .ToListAsync();
        }

        public async Task<IEnumerable<KmsAuditLog>> GetFilteredAsync(
            int? userId, string? action, string? tableName,
            DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.KmsAuditLogs
                .Include(l => l.User)
                .AsQueryable();

            if (userId.HasValue)
                query = query.Where(l => l.UserId == userId.Value);

            if (!string.IsNullOrEmpty(action))
                query = query.Where(l => l.Action == action);

            if (!string.IsNullOrEmpty(tableName))
                query = query.Where(l => l.TableName == tableName);

            if (fromDate.HasValue)
                query = query.Where(l => l.CreatedAt >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(l => l.CreatedAt <= toDate.Value.AddDays(1));

            return await query
                .OrderByDescending(l => l.CreatedAt)
                .Take(500)
                .ToListAsync();
        }

        public async Task<KmsAuditLog> CreateAsync(KmsAuditLog log)
        {
            _context.KmsAuditLogs.Add(log);
            await _context.SaveChangesAsync();
            return log;
        }
    }
}
