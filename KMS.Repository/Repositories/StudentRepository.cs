
using Microsoft.EntityFrameworkCore;
using KMS.Repository.Entities;
using KMS.Repository.Interfaces;

namespace KMS.Repository.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly ApplicationDbContext _context;

        public StudentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<KmsStudent?> GetByIdAsync(int studentId)
        {
            return await _context.KmsStudents
                .Include(s => s.KmsStudentParents)
                    .ThenInclude(sp => sp.Parent)
                        .ThenInclude(p => p.User)
                .Include(s => s.KmsClassStudents)
                    .ThenInclude(cs => cs.Class)
                .FirstOrDefaultAsync(s => s.StudentId == studentId);
        }

        public async Task<KmsStudent?> GetByStudentCodeAsync(string studentCode)
        {
            return await _context.KmsStudents
                .Include(s => s.KmsStudentParents)
                    .ThenInclude(sp => sp.Parent)
                        .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(s => s.StudentCode == studentCode);
        }

        public async Task<IEnumerable<KmsStudent>> GetAllAsync()
        {
            return await _context.KmsStudents
                .OrderBy(s => s.StudentCode)
                .ToListAsync();
        }

        public async Task<KmsStudent> CreateAsync(KmsStudent student)
        {
            student.CreatedAt = DateTime.Now;
            student.UpdatedAt = DateTime.Now;

            _context.KmsStudents.Add(student);
            await _context.SaveChangesAsync();
            return student;
        }

        public async Task UpdateAsync(KmsStudent student)
        {
            student.UpdatedAt = DateTime.Now;
            _context.KmsStudents.Update(student);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int studentId)
        {
            var student = await _context.KmsStudents.FindAsync(studentId);
            if (student != null)
            {
                _context.KmsStudents.Remove(student);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<KmsStudent>> GetActiveStudentsAsync()
        {
            return await _context.KmsStudents
                .Where(s => s.IsActive == true)
                .OrderBy(s => s.StudentCode)
                .ToListAsync();
        }

        public async Task<IEnumerable<KmsStudent>> GetInactiveStudentsAsync()
        {
            return await _context.KmsStudents
                .Where(s => s.IsActive != true)
                .OrderBy(s => s.StudentCode)
                .ToListAsync();
        }

        public async Task<IEnumerable<KmsStudent>> GetStudentsByClassIdAsync(int classId)
        {
            return await _context.KmsClassStudents
                .Where(cs => cs.ClassId == classId)
                .Select(cs => cs.Student)
                .OrderBy(s => s.StudentCode)
                .ToListAsync();
        }

        public async Task<IEnumerable<KmsStudent>> GetStudentsByParentIdAsync(int parentId)
        {
            return await _context.KmsStudentParents
                .Where(sp => sp.ParentId == parentId)
                .Select(sp => sp.Student)
                .OrderBy(s => s.StudentCode)
                .ToListAsync();
        }

        public async Task<IEnumerable<KmsStudent>> SearchStudentsAsync(string keyword)
        {
            return await _context.KmsStudents
                .Where(s => s.StudentCode.Contains(keyword) ||
                           s.FullName.Contains(keyword) ||
                           (s.Address != null && s.Address.Contains(keyword)))
                .OrderBy(s => s.StudentCode)
                .ToListAsync();
        }

        public async Task<bool> StudentCodeExistsAsync(string studentCode)
        {
            return await _context.KmsStudents
                .AnyAsync(s => s.StudentCode == studentCode);
        }

        public async Task<int> CountActiveStudentsAsync()
        {
            return await _context.KmsStudents
                .CountAsync(s => s.IsActive == true);
        }
    }
}