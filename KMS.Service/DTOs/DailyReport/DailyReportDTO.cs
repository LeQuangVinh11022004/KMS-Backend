using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.DTOs.DailyReport
{
    public class DailyReportDTO
    {
        public int ReportId { get; set; }

        public int StudentId { get; set; }

        public int ClassId { get; set; }

        public DateOnly? ReportDate { get; set; }

        public string LivingData { get; set; }

        public string EducationalData { get; set; }

        public string Status { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
    public class CreateDailyReportDTO
    {
        public int StudentId { get; set; }
        public int ClassId { get; set; }

        public string LivingData { get; set; }

        public int CreatedBy { get; set; }

        public string EducationalData { get; set; }

        public int? ReviewedBy { get; set; }

        public string Status { get; set; }
    }
    public class UpdateDailyReportDTO
    {
        public string LivingData { get; set; }

        public string EducationalData { get; set; }

        public int? ReviewedBy { get; set; }

        public string Status { get; set; }
    }
}
