using KMS.Repository.Entities;
using KMS.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KMS.Repository.Repositories
{
    public class StudentParentRepository : IStudentParentRepository
    {
        private readonly ApplicationDbContext _context;

        public StudentParentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<KmsStudentParent>> GetParentsByStudentAsync(int studentId)
        {
            return await _context.KmsStudentParents
                .Include(sp => sp.Student)
                .Include(sp => sp.Parent)
                    .ThenInclude(p => p.User)
                .Where(sp => sp.StudentId == studentId)
                .OrderByDescending(sp => sp.IsPrimaryContact)
                .ThenBy(sp => sp.Relationship)
                .ToListAsync();
        }

        public async Task<IEnumerable<KmsStudentParent>> GetStudentsByParentAsync(int parentId)
        {
            return await _context.KmsStudentParents
                .Include(sp => sp.Student)
                .Include(sp => sp.Parent)
                    .ThenInclude(p => p.User)
                .Where(sp => sp.ParentId == parentId)
                .OrderBy(sp => sp.Student.FullName)
                .ToListAsync();
        }

        public async Task<KmsStudentParent?> GetAsync(int studentId, int parentId)
        {
            return await _context.KmsStudentParents
                .Include(sp => sp.Student)
                .Include(sp => sp.Parent)
                    .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(sp => sp.StudentId == studentId && sp.ParentId == parentId);
        }

        public async Task<bool> LinkExistsAsync(int studentId, int parentId)
        {
            return await _context.KmsStudentParents
                .AnyAsync(sp => sp.StudentId == studentId && sp.ParentId == parentId);
        }

        public async Task<bool> HasPrimaryContactAsync(int studentId)
        {
            return await _context.KmsStudentParents
                .AnyAsync(sp => sp.StudentId == studentId && sp.IsPrimaryContact == true);
        }

        public async Task<KmsStudentParent> LinkAsync(KmsStudentParent studentParent)
        {
            _context.KmsStudentParents.Add(studentParent);
            await _context.SaveChangesAsync();
            await _context.Entry(studentParent).Reference(sp => sp.Student).LoadAsync();
            await _context.Entry(studentParent).Reference(sp => sp.Parent).Query()
                .Include(p => p.User).LoadAsync();
            return studentParent;
        }

        public async Task UpdateAsync(KmsStudentParent studentParent)
        {
            _context.KmsStudentParents.Update(studentParent);
            await _context.SaveChangesAsync();
        }

        public async Task UnlinkAsync(int studentId, int parentId)
        {
            var record = await _context.KmsStudentParents
                .FirstOrDefaultAsync(sp => sp.StudentId == studentId && sp.ParentId == parentId);
            if (record != null)
            {
                _context.KmsStudentParents.Remove(record);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UnlinkAllParentsFromStudentAsync(int studentId)
        {
            var records = await _context.KmsStudentParents
                .Where(sp => sp.StudentId == studentId)
                .ToListAsync();
            _context.KmsStudentParents.RemoveRange(records);
            await _context.SaveChangesAsync();
        }
    }
}