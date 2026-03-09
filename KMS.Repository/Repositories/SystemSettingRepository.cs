using KMS.Repository.Entities;
using KMS.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KMS.Repository.Repositories
{

    public class SystemSettingRepository : ISystemSettingRepository
    {
        private readonly ApplicationDbContext _context;

        public SystemSettingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<KmsSystemSetting>> GetAllAsync()
        {
            return await _context.KmsSystemSettings
                .Include(s => s.UpdatedByNavigation)
                .OrderBy(s => s.SettingKey)
                .ToListAsync();
        }

        public async Task<KmsSystemSetting?> GetByKeyAsync(string key)
        {
            return await _context.KmsSystemSettings
                .Include(s => s.UpdatedByNavigation)
                .FirstOrDefaultAsync(s => s.SettingKey == key);
        }

        public async Task<KmsSystemSetting?> GetByIdAsync(int settingId)
        {
            return await _context.KmsSystemSettings
                .Include(s => s.UpdatedByNavigation)
                .FirstOrDefaultAsync(s => s.SettingId == settingId);
        }

        public async Task UpdateAsync(KmsSystemSetting setting)
        {
            _context.KmsSystemSettings.Update(setting);
            await _context.SaveChangesAsync();
        }
    }
}