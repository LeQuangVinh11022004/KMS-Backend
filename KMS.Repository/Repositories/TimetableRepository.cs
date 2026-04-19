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
    public class TimetableRepository : ITimetableRepository
    {
        private readonly ApplicationDbContext _context;

        public TimetableRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<KmsTimetable>> GetByClassIdAsync(int classId)
        {
            return await _context.KmsTimetables
                .Where(t => t.ClassId == classId && t.IsActive == true)
                .OrderBy(t => t.DayOfWeek).ThenBy(t => t.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<KmsTimetable>> GetByTeacherIdAsync(int teacherId)
        {
            return await _context.KmsTimetables
                .Where(t => t.TeacherId == teacherId && t.IsActive == true)
                .OrderBy(t => t.DayOfWeek).ThenBy(t => t.StartTime)
                .ToListAsync();
        }

        public async Task<KmsTimetable?> GetByIdAsync(int id)
        {
            return await _context.KmsTimetables.FirstOrDefaultAsync(t => t.TimetableId == id);
        }

        public async Task CreateAsync(KmsTimetable timetable)
        {
            await _context.KmsTimetables.AddAsync(timetable);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(KmsTimetable timetable)
        {
            _context.KmsTimetables.Update(timetable);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(KmsTimetable timetable)
        {
            _context.KmsTimetables.Remove(timetable);
            await _context.SaveChangesAsync();
        }
    }
}
