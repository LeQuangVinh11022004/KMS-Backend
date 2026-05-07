using KMS.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text.Json;

namespace KMS.Service.Plugins
{
    public class DailyReportPlugin
    {
        private readonly ApplicationDbContext _context;

        public DailyReportPlugin(ApplicationDbContext context)
        {
            _context = context;
        }

        [KernelFunction("get_daily_report")]
        [Description("Lấy báo cáo sinh hoạt hôm nay của một học sinh theo StudentId")]
        public async Task<string> GetDailyReportAsync(
            [Description("ID của học sinh cần xem báo cáo sinh hoạt")] int studentId)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            var report = await _context.KmsDailyReports
                .FirstOrDefaultAsync(x => x.StudentId == studentId
                                       && x.ReportDate == today
                                       && x.Status == "Published");

            if (report == null)
            {
                return JsonSerializer.Serialize(new { studentId = studentId, hasData = false, message = "no_data" });
            }

            return JsonSerializer.Serialize(new { studentId = studentId, hasData = true, livingData = report.LivingData });
        }
    }
}