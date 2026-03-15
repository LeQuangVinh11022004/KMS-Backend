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
    public class NotificationRepository : INotificationRepository
    {
        private readonly ApplicationDbContext _context;

        public NotificationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<KmsNotification>> GetByUserIdAsync(int userId)
        {
            return await _context.KmsNotifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<KmsNotification?> GetByIdAsync(int id)
        {
            return await _context.KmsNotifications
                .FirstOrDefaultAsync(n => n.NotificationId == id);
        }

        public async Task CreateAsync(KmsNotification notification)
        {
            await _context.KmsNotifications.AddAsync(notification);
        }

        public async Task CreateBulkAsync(IEnumerable<KmsNotification> notifications)
        {
            await _context.KmsNotifications.AddRangeAsync(notifications);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(KmsNotification notification)
        {
            _context.KmsNotifications.Update(notification);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(KmsNotification notification)
        {
            _context.KmsNotifications.Remove(notification);
            await _context.SaveChangesAsync();
        }
    }
}
