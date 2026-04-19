using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.DTOs.Timetable
{
    public class TimetableDTO
    {
        public int TimetableId { get; set; }
        public int ClassId { get; set; }
        public int DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string Subject { get; set; } = null!;
        public int? TeacherId { get; set; }
        public string Room { get; set; } = null!;
        public bool IsActive { get; set; }
    }

    public class CreateTimetableDTO
    {
        public int ClassId { get; set; }
        public int DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string Subject { get; set; } = null!;
        public int? TeacherId { get; set; }
        public string Room { get; set; } = null!;
    }

    public class UpdateTimetableDTO
    {
        public int DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string Subject { get; set; } = null!;
        public int? TeacherId { get; set; }
        public string Room { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
