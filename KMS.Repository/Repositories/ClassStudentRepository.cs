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
    public class ClassStudentRepository : IClassStudentRepository
    {
        private readonly ApplicationDbContext _context;

        public ClassStudentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<KmsClassStudent>> GetStudentsByClassAsync(int classId)
        {
            return await _context.KmsClassStudents
                .Include(cs => cs.Student)
                .Include(cs => cs.Class)
                .Include(cs => cs.EnrolledByNavigation)
                .Where(cs => cs.ClassId == classId)
                .OrderBy(cs => cs.Student.FullName)
                .ToListAsync();
        }

        public async Task<IEnumerable<KmsClassStudent>> GetClassesByStudentAsync(int studentId)
        {
            return await _context.KmsClassStudents
                .Include(cs => cs.Class)
                    .ThenInclude(c => c.SchoolYear)
                .Include(cs => cs.Student)
                .Where(cs => cs.StudentId == studentId)
                .OrderByDescending(cs => cs.EnrolledAt)
                .ToListAsync();
        }

        public async Task<KmsClassStudent?> GetAsync(int classId, int studentId)
        {
            return await _context.KmsClassStudents
                .Include(cs => cs.Student)
                .Include(cs => cs.Class)
                .FirstOrDefaultAsync(cs => cs.ClassId == classId && cs.StudentId == studentId);
        }

        public async Task<bool> IsStudentInClassAsync(int classId, int studentId)
        {
            return await _context.KmsClassStudents
                .AnyAsync(cs => cs.ClassId == classId && cs.StudentId == studentId);
        }

        public async Task<bool> IsStudentInAnyActiveClassAsync(int studentId)
        {
            return await _context.KmsClassStudents
                .AnyAsync(cs => cs.StudentId == studentId && cs.Class.IsActive == true);
        }

        public async Task<int> CountStudentsInClassAsync(int classId)
        {
            return await _context.KmsClassStudents
                .CountAsync(cs => cs.ClassId == classId);
        }

        public async Task<KmsClassStudent> EnrollAsync(KmsClassStudent classStudent)
        {
            _context.KmsClassStudents.Add(classStudent);
            await _context.SaveChangesAsync();
            await _context.Entry(classStudent).Reference(cs => cs.Student).LoadAsync();
            await _context.Entry(classStudent).Reference(cs => cs.Class).LoadAsync();

            // Load parents → Parent → User để lấy email
            await _context.Entry(classStudent.Student)
                .Collection(s => s.KmsStudentParents)
                .Query()
                .Include(sp => sp.Parent)
                    .ThenInclude(p => p.User)
                    .ThenInclude(p => p.KmsClasses)
                .LoadAsync();

            return classStudent;
        }

        public async Task RemoveAsync(int classId, int studentId)
        {
            var record = await _context.KmsClassStudents
                .FirstOrDefaultAsync(cs => cs.ClassId == classId && cs.StudentId == studentId);
            if (record != null)
            {
                _context.KmsClassStudents.Remove(record);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveAllStudentsFromClassAsync(int classId)
        {
            var records = await _context.KmsClassStudents
                .Where(cs => cs.ClassId == classId)
                .ToListAsync();
            _context.KmsClassStudents.RemoveRange(records);
            await _context.SaveChangesAsync();
        }
    }
}
