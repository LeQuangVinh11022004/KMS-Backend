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
    public class ClassActivityRepository : IClassActivityRepository
    {
        private readonly ApplicationDbContext _context;

        public ClassActivityRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<KmsClassActivity>> GetAllAsync()
        {
            return await _context.KmsClassActivities
                .Include(x => x.KmsActivityPhotos)
                .ToListAsync();
        }

        public async Task<List<KmsClassActivity>> GetByClassIdAsync(int classId)
        {
            return await _context.KmsClassActivities
                .Where(x => x.ClassId == classId)
                .Include(x => x.KmsActivityPhotos)
                .ToListAsync();
        }

        public async Task<KmsClassActivity?> GetByIdAsync(int id)
        {
            return await _context.KmsClassActivities
                .Include(x => x.KmsActivityPhotos)
                .FirstOrDefaultAsync(x => x.ActivityId == id);
        }

        public async Task AddAsync(KmsClassActivity entity)
        {
            await _context.KmsClassActivities.AddAsync(entity);
        }

        public void Update(KmsClassActivity entity)
        {
            _context.KmsClassActivities.Update(entity);
        }

        public void Delete(KmsClassActivity entity)
        {
            _context.KmsClassActivities.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
