using KMS.Repository.Entities;
using KMS.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KMS.Repository.Repositories
{
    public class ParentRegistrationRepository : IParentRegistrationRepository
    {
        private readonly ApplicationDbContext _context;

        public ParentRegistrationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // READ
        // ============================================================

        public async Task<KmsParentRegistration?> GetByIdAsync(int registrationId)
        {
            return await _context.KmsParentRegistrations
                .Include(r => r.ReviewedByNavigation)
                .FirstOrDefaultAsync(r => r.RegistrationId == registrationId);
        }

        public async Task<IEnumerable<KmsParentRegistration>> GetAllAsync()
        {
            return await _context.KmsParentRegistrations
                .Include(r => r.ReviewedByNavigation)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<KmsParentRegistration>> GetByStatusAsync(string status)
        {
            return await _context.KmsParentRegistrations
                .Include(r => r.ReviewedByNavigation)
                .Where(r => r.Status == status)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> EmailAlreadyRegisteredAsync(string email)
        {
            // Kiểm tra email đã có tài khoản User chưa
            return await _context.KmsUsers
                .AnyAsync(u => u.Email == email);
        }

        public async Task<bool> EmailAlreadyPendingAsync(string email)
        {
            // Kiểm tra email đã có đơn Pending chưa (tránh submit trùng)
            return await _context.KmsParentRegistrations
                .AnyAsync(r => r.Email == email && r.Status == "Pending");
        }

        // ============================================================
        // CREATE
        // ============================================================

        public async Task<KmsParentRegistration> CreateAsync(KmsParentRegistration registration)
        {
            _context.KmsParentRegistrations.Add(registration);
            await _context.SaveChangesAsync();
            return registration;
        }

        // ============================================================
        // UPDATE
        // ============================================================

        public async Task UpdateAsync(KmsParentRegistration registration)
        {
            _context.KmsParentRegistrations.Update(registration);
            await _context.SaveChangesAsync();
        }

        // ============================================================
        // STATS
        // ============================================================

        public async Task<int> CountByStatusAsync(string status)
        {
            return await _context.KmsParentRegistrations
                .CountAsync(r => r.Status == status);
        }
    }
}