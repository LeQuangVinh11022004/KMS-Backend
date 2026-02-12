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
    public class AnnouncementRepository : IAnnouncementRepository
    {
        private readonly ApplicationDbContext _context;

        public AnnouncementRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<KmsAnnouncement>> GetAllAsync()
        {
            return await _context.KmsAnnouncements
                .Include(x => x.TargetClass)
                .ToListAsync();
        }

        public async Task<KmsAnnouncement?> GetByIdAsync(int id)
        {
            return await _context.KmsAnnouncements
                .FirstOrDefaultAsync(x => x.AnnouncementId == id);
        }

        public async Task AddAsync(KmsAnnouncement entity)
        {
            await _context.KmsAnnouncements.AddAsync(entity);
        }

        public void Update(KmsAnnouncement entity)
        {
            _context.KmsAnnouncements.Update(entity);
        }

        public void Delete(KmsAnnouncement entity)
        {
            _context.KmsAnnouncements.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
