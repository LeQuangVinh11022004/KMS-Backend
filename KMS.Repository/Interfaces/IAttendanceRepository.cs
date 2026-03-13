using KMS.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Repository.Interfaces
{
    public interface IAttendanceRepository
    {
        // ── Lấy danh sách học sinh của lớp theo ngày ─────────────
        // Trả về tất cả học sinh trong lớp + attendance record nếu đã có
        Task<IEnumerable<KmsStudent>> GetStudentsByClassAsync(int classId);
        Task<IEnumerable<KmsAttendance>> GetByClassAndDateAsync(int classId, DateOnly date);

        // ── CRUD ──────────────────────────────────────────────────
        Task<KmsAttendance?> GetByIdAsync(int attendanceId);
        Task<KmsAttendance?> GetByStudentAndDateAsync(int studentId, DateOnly date);
        Task<KmsAttendance> CreateAsync(KmsAttendance attendance);
        Task CreateBulkAsync(IEnumerable<KmsAttendance> attendances);
        Task UpdateAsync(KmsAttendance attendance);

        // ── Lịch sử ───────────────────────────────────────────────
        Task<IEnumerable<KmsAttendance>> GetByStudentAsync(
            int studentId, DateOnly? fromDate, DateOnly? toDate);

        // ── Kiểm tra ──────────────────────────────────────────────
        Task<bool> ExistsAsync(int studentId, DateOnly date);
        Task<bool> IsClassAttendanceTakenAsync(int classId, DateOnly date);
    }
}
