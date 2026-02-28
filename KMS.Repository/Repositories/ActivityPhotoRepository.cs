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
    public class ActivityPhotoRepository : IActivityPhotoRepository
    {
        private readonly ApplicationDbContext _context;

        public ActivityPhotoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<KmsActivityPhoto>> GetByActivityIdAsync(int activityId)
        {
            return await _context.KmsActivityPhotos
                .Where(x => x.ActivityId == activityId)
                .ToListAsync();
        }

        public async Task<KmsActivityPhoto?> GetByIdAsync(int photoId)
        {
            return await _context.KmsActivityPhotos
                .FirstOrDefaultAsync(x => x.PhotoId == photoId);
        }

        public async Task AddAsync(KmsActivityPhoto entity)
        {
            await _context.KmsActivityPhotos.AddAsync(entity);
        }

        public void Delete(KmsActivityPhoto entity)
        {
            _context.KmsActivityPhotos.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
