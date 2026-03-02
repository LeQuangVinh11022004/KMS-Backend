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
    public class ClassTeacherRepository : IClassTeacherRepository
    {
        private readonly ApplicationDbContext _context;

        public ClassTeacherRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<KmsClassTeacher>> GetTeachersByClassAsync(int classId)
        {
            return await _context.KmsClassTeachers
                .Include(ct => ct.Teacher)
                    .ThenInclude(t => t.User)
                .Include(ct => ct.Class)
                .Include(ct => ct.AssignedByNavigation)
                .Where(ct => ct.ClassId == classId)
                .OrderBy(ct => ct.Role) // Main first
                .ToListAsync();
        }

        public async Task<IEnumerable<KmsClassTeacher>> GetClassesByTeacherAsync(int teacherId)
        {
            return await _context.KmsClassTeachers
                .Include(ct => ct.Class)
                    .ThenInclude(c => c.SchoolYear)
                .Include(ct => ct.Teacher)
                    .ThenInclude(t => t.User)
                .Where(ct => ct.TeacherId == teacherId)
                .OrderByDescending(ct => ct.AssignedAt)
                .ToListAsync();
        }

        public async Task<KmsClassTeacher?> GetAsync(int classId, int teacherId)
        {
            return await _context.KmsClassTeachers
                .Include(ct => ct.Teacher)
                    .ThenInclude(t => t.User)
                .Include(ct => ct.Class)
                .FirstOrDefaultAsync(ct => ct.ClassId == classId && ct.TeacherId == teacherId);
        }

        public async Task<bool> IsTeacherInClassAsync(int classId, int teacherId)
        {
            return await _context.KmsClassTeachers
                .AnyAsync(ct => ct.ClassId == classId && ct.TeacherId == teacherId);
        }

        public async Task<bool> ClassHasMainTeacherAsync(int classId)
        {
            return await _context.KmsClassTeachers
                .AnyAsync(ct => ct.ClassId == classId && ct.Role == "Main");
        }

        public async Task<int> CountTeachersInClassAsync(int classId)
        {
            return await _context.KmsClassTeachers
                .CountAsync(ct => ct.ClassId == classId);
        }

        public async Task<KmsClassTeacher> AssignAsync(KmsClassTeacher classTeacher)
        {
            _context.KmsClassTeachers.Add(classTeacher);
            await _context.SaveChangesAsync();
            await _context.Entry(classTeacher).Reference(ct => ct.Teacher).LoadAsync();
            await _context.Entry(classTeacher).Reference(ct => ct.Class).LoadAsync();
            return classTeacher;
        }

        public async Task UpdateAsync(KmsClassTeacher classTeacher)
        {
            _context.KmsClassTeachers.Update(classTeacher);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(int classId, int teacherId)
        {
            var record = await _context.KmsClassTeachers
                .FirstOrDefaultAsync(ct => ct.ClassId == classId && ct.TeacherId == teacherId);
            if (record != null)
            {
                _context.KmsClassTeachers.Remove(record);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveAllTeachersFromClassAsync(int classId)
        {
            var records = await _context.KmsClassTeachers
                .Where(ct => ct.ClassId == classId)
                .ToListAsync();
            _context.KmsClassTeachers.RemoveRange(records);
            await _context.SaveChangesAsync();
        }
    }
}

