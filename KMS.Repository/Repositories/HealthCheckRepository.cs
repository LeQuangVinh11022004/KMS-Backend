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
    public class HealthCheckRepository : IHealthCheckRepository
    {
        private readonly ApplicationDbContext _context;

        public HealthCheckRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<KmsHealthCheck>> GetAllAsync()
            => await _context.KmsHealthChecks
                .Include(x => x.Student)
                .ToListAsync();

        public async Task<KmsHealthCheck?> GetByIdAsync(int id)
            => await _context.KmsHealthChecks
                .Include(x => x.Student)
                .FirstOrDefaultAsync(x => x.CheckId == id);

        public async Task<List<KmsHealthCheck>> GetByStudentIdAsync(int studentId)
            => await _context.KmsHealthChecks
                .Where(x => x.StudentId == studentId)
                .ToListAsync();

        public async Task AddAsync(KmsHealthCheck entity)
        {
            await _context.KmsHealthChecks.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(KmsHealthCheck entity)
        {
            _context.KmsHealthChecks.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(KmsHealthCheck entity)
        {
            _context.KmsHealthChecks.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
