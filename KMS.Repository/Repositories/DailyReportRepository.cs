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
    public class DailyReportRepository : IDailyReportRepository
    {
        private readonly ApplicationDbContext _context;

        public DailyReportRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<KmsDailyReport>> GetAllAsync()
        {
            return await _context.KmsDailyReports
                .Include(x => x.Student)
                .Include(x => x.Class)
                .OrderByDescending(x => x.ReportDate)
                .ToListAsync();
        }

        public async Task<KmsDailyReport?> GetByIdAsync(int reportId)
        {
            return await _context.KmsDailyReports
                .Include(x => x.Student)
                .Include(x => x.Class)
                .FirstOrDefaultAsync(x => x.ReportId == reportId);
        }

        public async Task AddAsync(KmsDailyReport entity)
        {
            await _context.KmsDailyReports.AddAsync(entity);
        }

        public void Update(KmsDailyReport entity)
        {
            _context.KmsDailyReports.Update(entity);
        }

        public void Delete(KmsDailyReport entity)
        {
            _context.KmsDailyReports.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
