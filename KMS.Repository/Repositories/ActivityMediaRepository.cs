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
    public class ActivityMediaRepository : IActivityMediaRepository
    {
        private readonly ApplicationDbContext _context;

        public ActivityMediaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<KmsActivityMedium>> GetByActivityIdAsync(int activityId)
        {
            return await _context.KmsActivityMedia
                .Where(x => x.ActivityId == activityId)
                .OrderByDescending(x => x.UploadedAt)
                .ToListAsync();
        }

        public async Task<KmsActivityMedium?> GetByIdAsync(int mediaId)
        {
            return await _context.KmsActivityMedia
                .FirstOrDefaultAsync(x => x.MediaId == mediaId);
        }

        public async Task AddAsync(KmsActivityMedium entity)
        {
            await _context.KmsActivityMedia.AddAsync(entity);
        }

        public void Delete(KmsActivityMedium entity)
        {
            _context.KmsActivityMedia.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}