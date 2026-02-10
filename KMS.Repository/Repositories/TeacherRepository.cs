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
        public class TeacherRepository : ITeacherRepository
        {
            private readonly ApplicationDbContext _context;

            public TeacherRepository(ApplicationDbContext context)
            {
                _context = context;
            }

            // ============================================================
            // READ
            // ============================================================

            public async Task<KmsTeacher?> GetByIdAsync(int teacherId)
            {
                return await _context.KmsTeachers
                    .Include(t => t.User)
                    .FirstOrDefaultAsync(t => t.TeacherId == teacherId);
            }

            public async Task<KmsTeacher?> GetByUserIdAsync(int userId)
            {
                return await _context.KmsTeachers
                    .Include(t => t.User)
                    .FirstOrDefaultAsync(t => t.UserId == userId);
            }

            public async Task<KmsTeacher?> GetByTeacherCodeAsync(string teacherCode)
            {
                return await _context.KmsTeachers
                    .Include(t => t.User)
                    .FirstOrDefaultAsync(t => t.TeacherCode == teacherCode);
            }

            public async Task<IEnumerable<KmsTeacher>> GetAllAsync()
            {
                return await _context.KmsTeachers
                    .Include(t => t.User)
                    .OrderBy(t => t.TeacherCode)
                    .ToListAsync();
            }

            // ============================================================
            // CREATE
            // ============================================================

            public async Task<KmsTeacher> CreateAsync(KmsTeacher teacher)
            {
                _context.KmsTeachers.Add(teacher);
                await _context.SaveChangesAsync();

                // Load navigation properties
                await _context.Entry(teacher).Reference(t => t.User).LoadAsync();

                return teacher;
            }

            // ============================================================
            // UPDATE
            // ============================================================

            public async Task UpdateAsync(KmsTeacher teacher)
            {
                _context.KmsTeachers.Update(teacher);
                await _context.SaveChangesAsync();
            }

            // ============================================================
            // DELETE
            // ============================================================

            public async Task DeleteAsync(int teacherId)
            {
                var teacher = await _context.KmsTeachers.FindAsync(teacherId);
                if (teacher != null)
                {
                    _context.KmsTeachers.Remove(teacher);
                    await _context.SaveChangesAsync();
                }
            }

            // ============================================================
            // FILTER
            // ============================================================

            public async Task<IEnumerable<KmsTeacher>> GetActiveTeachersAsync()
            {
                return await _context.KmsTeachers
                    .Include(t => t.User)
                    .Where(t => t.IsActive == true)
                    .OrderBy(t => t.TeacherCode)
                    .ToListAsync();
            }

            public async Task<IEnumerable<KmsTeacher>> GetInactiveTeachersAsync()
            {
                return await _context.KmsTeachers
                    .Include(t => t.User)
                    .Where(t => t.IsActive != true)
                    .OrderBy(t => t.TeacherCode)
                    .ToListAsync();
            }

            public async Task<IEnumerable<KmsTeacher>> SearchTeachersAsync(string keyword)
            {
                return await _context.KmsTeachers
                    .Include(t => t.User)
                    .Where(t => t.TeacherCode.Contains(keyword) ||
                               t.User.FullName.Contains(keyword) ||
                               (t.Specialization != null && t.Specialization.Contains(keyword)))
                    .OrderBy(t => t.TeacherCode)
                    .ToListAsync();
            }

            // ============================================================
            // CHECK
            // ============================================================

            public async Task<bool> TeacherCodeExistsAsync(string teacherCode)
            {
                return await _context.KmsTeachers
                    .AnyAsync(t => t.TeacherCode == teacherCode);
            }

            public async Task<bool> UserIsTeacherAsync(int userId)
            {
                return await _context.KmsTeachers
                    .AnyAsync(t => t.UserId == userId);
            }

            // ============================================================
            // STATISTICS
            // ============================================================

            public async Task<int> CountActiveTeachersAsync()
            {
                return await _context.KmsTeachers
                    .CountAsync(t => t.IsActive == true);
            }
        }
    }

