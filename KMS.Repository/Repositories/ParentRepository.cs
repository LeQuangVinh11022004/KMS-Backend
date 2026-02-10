using KMS.Repository.Entities;
using KMS.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

    namespace KMS.Repository.Repositories
    {
        public class ParentRepository : IParentRepository
        {
            private readonly ApplicationDbContext _context;

            public ParentRepository(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<KmsParent?> GetByIdAsync(int parentId)
            {
                return await _context.KmsParents
                    .Include(p => p.User)
                    .Include(p => p.KmsStudentParents)
                        .ThenInclude(sp => sp.Student)
                    .FirstOrDefaultAsync(p => p.ParentId == parentId);
            }

            public async Task<KmsParent?> GetByUserIdAsync(int userId)
            {
                return await _context.KmsParents
                    .Include(p => p.User)
                    .Include(p => p.KmsStudentParents)
                        .ThenInclude(sp => sp.Student)
                    .FirstOrDefaultAsync(p => p.UserId == userId);
            }

            public async Task<IEnumerable<KmsParent>> GetAllAsync()
            {
                return await _context.KmsParents
                    .Include(p => p.User)
                    .OrderBy(p => p.User.FullName)
                    .ToListAsync();
            }

            public async Task<KmsParent> CreateAsync(KmsParent parent)
            {
                _context.KmsParents.Add(parent);
                await _context.SaveChangesAsync();
                await _context.Entry(parent).Reference(p => p.User).LoadAsync();
                return parent;
            }

            public async Task UpdateAsync(KmsParent parent)
            {
                _context.KmsParents.Update(parent);
                await _context.SaveChangesAsync();
            }

            public async Task DeleteAsync(int parentId)
            {
                var parent = await _context.KmsParents.FindAsync(parentId);
                if (parent != null)
                {
                    _context.KmsParents.Remove(parent);
                    await _context.SaveChangesAsync();
                }
            }

            public async Task<IEnumerable<KmsParent>> GetParentsByStudentIdAsync(int studentId)
            {
                return await _context.KmsStudentParents
                    .Where(sp => sp.StudentId == studentId)
                    .Select(sp => sp.Parent)
                    .Include(p => p.User)
                    .ToListAsync();
            }

            public async Task<IEnumerable<KmsParent>> SearchParentsAsync(string keyword)
            {
                return await _context.KmsParents
                    .Include(p => p.User)
                    .Where(p => p.User.FullName.Contains(keyword) ||
                               p.User.Email.Contains(keyword) ||
                               (p.Occupation != null && p.Occupation.Contains(keyword)))
                    .OrderBy(p => p.User.FullName)
                    .ToListAsync();
            }

            public async Task<bool> UserIsParentAsync(int userId)
            {
                return await _context.KmsParents.AnyAsync(p => p.UserId == userId);
            }
        }
    }
