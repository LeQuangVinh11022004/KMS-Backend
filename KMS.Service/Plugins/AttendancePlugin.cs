using KMS.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text.Json;

namespace KMS.Service.Plugins
{
    public class AttendancePlugin
    {
        private readonly ApplicationDbContext _context;

        public AttendancePlugin(ApplicationDbContext context)
        {
            _context = context;
        }

        [KernelFunction("get_attendance")]
        [Description("Lấy thông tin điểm danh hôm nay của một học sinh theo StudentId")]
        public async Task<string> GetAttendanceAsync(
            [Description("ID của học sinh cần tra cứu điểm danh")] int studentId)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            var attendance = await _context.KmsAttendances
                .FirstOrDefaultAsync(x => x.StudentId == studentId
                                       && x.AttendanceDate == today);

            if (attendance == null)
            {
                return JsonSerializer.Serialize(new { studentId = studentId, hasData = false, message = "no_data" });
            }

            return JsonSerializer.Serialize(new { studentId = studentId, hasData = true, status = attendance.Status });
        }
    }
}