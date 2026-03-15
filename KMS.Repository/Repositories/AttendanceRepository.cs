using KMS.Repository.Entities;
using KMS.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KMS.Repository.Repositories
{
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly ApplicationDbContext _context;

        public AttendanceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // ── Lấy học sinh của lớp ──────────────────────────────────

        public async Task<IEnumerable<KmsStudent>> GetStudentsByClassAsync(int classId)
        {
            return await _context.KmsClassStudents
                .Where(cs => cs.ClassId == classId)
                .Include(cs => cs.Student)
                .Where(cs => cs.Student.IsActive == true)
                .OrderBy(cs => cs.Student.FullName)
                .Select(cs => cs.Student)
                .ToListAsync();
        }

        public async Task<IEnumerable<KmsAttendance>> GetByClassAndDateAsync(int classId, DateOnly date)
        {
            return await _context.KmsAttendances
                .Include(a => a.Student)
                .Include(a => a.TakenByNavigation)
                    .ThenInclude(t => t.User)
                .Include(a => a.UpdatedByNavigation)
                    .ThenInclude(t => t.User)
                .Where(a => a.ClassId == classId && a.AttendanceDate == date)
                .ToListAsync();
        }

        // ── CRUD ──────────────────────────────────────────────────

        public async Task<KmsAttendance?> GetByIdAsync(int attendanceId)
        {
            return await _context.KmsAttendances
                .Include(a => a.Student)
                .Include(a => a.Class)
                .Include(a => a.TakenByNavigation).ThenInclude(t => t.User)
                .Include(a => a.UpdatedByNavigation).ThenInclude(t => t.User)
                .FirstOrDefaultAsync(a => a.AttendanceId == attendanceId);
        }

        public async Task<KmsAttendance?> GetByStudentAndDateAsync(int studentId, DateOnly date)
        {
            return await _context.KmsAttendances
                .FirstOrDefaultAsync(a => a.StudentId == studentId && a.AttendanceDate == date);
        }

        public async Task<KmsAttendance> CreateAsync(KmsAttendance attendance)
        {
            _context.KmsAttendances.Add(attendance);
            await _context.SaveChangesAsync();
            return attendance;
        }

        public async Task CreateBulkAsync(IEnumerable<KmsAttendance> attendances)
        {
            _context.KmsAttendances.AddRange(attendances);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(KmsAttendance attendance)
        {
            _context.KmsAttendances.Update(attendance);
            await _context.SaveChangesAsync();
        }

        // ── Lịch sử ───────────────────────────────────────────────

        public async Task<IEnumerable<KmsAttendance>> GetByStudentAsync(
            int studentId, DateOnly? fromDate, DateOnly? toDate)
        {
            var query = _context.KmsAttendances
                .Include(a => a.Class)
                .Include(a => a.TakenByNavigation).ThenInclude(t => t.User)
                .Where(a => a.StudentId == studentId)
                .AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(a => a.AttendanceDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(a => a.AttendanceDate <= toDate.Value);

            return await query
                .OrderByDescending(a => a.AttendanceDate)
                .ToListAsync();
        }

        // ── Kiểm tra ──────────────────────────────────────────────

        public async Task<bool> ExistsAsync(int studentId, DateOnly date)
        {
            return await _context.KmsAttendances
                .AnyAsync(a => a.StudentId == studentId && a.AttendanceDate == date);
        }

        public async Task<bool> IsClassAttendanceTakenAsync(int classId, DateOnly date)
        {
            return await _context.KmsAttendances
                .AnyAsync(a => a.ClassId == classId && a.AttendanceDate == date);
        }

        public async Task<IEnumerable<KmsAttendance>> GetByClassAndDateForStudentsAsync(int classId, DateOnly date, IEnumerable<int> studentIds)
        {
            return await _context.KmsAttendances
                .Where(a =>
                    a.ClassId == classId &&
                    a.AttendanceDate == date &&
                    studentIds.Contains(a.StudentId))
                .ToListAsync();
        }
    }
}