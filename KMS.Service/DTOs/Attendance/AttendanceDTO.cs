using System.ComponentModel;
namespace KMS.Service.DTOs.Attendance
{
    // ============================================================
    // RESPONSE DTOs
    // ============================================================

    public class AttendanceDTO
    {
        public int AttendanceId { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; } = null!;
        public string? StudentCode { get; set; }
        public string? StudentPhoto { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; } = null!;
        public DateTime AttendanceDate { get; set; }
        public string Status { get; set; } = null!;   // Present, Absent, Late, Excused, Sick
        public TimeOnly? CheckInTime { get; set; }
        public TimeOnly? CheckOutTime { get; set; }
        public string? Notes { get; set; }
        public string? TakenByName { get; set; }
        public DateTime? TakenAt { get; set; }
        public string? UpdatedByName { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    // Dùng cho Teacher mở danh sách lớp theo ngày
    // Trả về toàn bộ học sinh, kèm trạng thái điểm danh nếu đã có
    public class ClassAttendanceSheetDTO
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; } = null!;
        public DateTime Date { get; set; }
        public bool IsSubmitted { get; set; }  // true nếu đã điểm danh hôm nay
        public int TotalStudents { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public int LateCount { get; set; }
        public int SickCount { get; set; }
        public int ExcusedCount { get; set; }
        public List<StudentAttendanceRowDTO> Students { get; set; } = new();
    }

    public class StudentAttendanceRowDTO
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = null!;
        public string? StudentCode { get; set; }
        public string? StudentPhoto { get; set; }
        public string? Gender { get; set; }

        // null = chưa điểm danh, có giá trị = đã điểm danh
        public int? AttendanceId { get; set; }
        public string? Status { get; set; }
        public TimeOnly? CheckInTime { get; set; }
        public TimeOnly? CheckOutTime { get; set; }
        public string? Notes { get; set; }
    }

    // ============================================================
    // REQUEST DTOs
    // ============================================================

    // Điểm danh 1 học sinh
    public class AttendanceRecordDTO
    {
        [DefaultValue(1)]
        public int StudentId { get; set; }

        /// <summary>Present | Absent | Late | Excused | Sick</summary>
        [DefaultValue("Present")]
        public string Status { get; set; } = null!;

        // CheckInTime KHÔNG nhận từ request
        // → Hệ thống tự set = DateTime.Now khi Teacher bấm lưu điểm danh

        // CheckOutTime KHÔNG có lúc điểm danh buổi sáng
        // → Teacher cập nhật sau qua PUT /{attendanceId}/checkout

        /// <summary>Ghi chú (ví dụ: Bé sốt, đón sớm...)</summary>
        [DefaultValue(null)]
        public string? Notes { get; set; }
    }

    // Lưu điểm danh cả lớp 1 lần (bulk)
    public class BulkAttendanceDTO
    {
        [DefaultValue(1)]
        public int ClassId { get; set; }

        /// <summary>Ngày điểm danh (ví dụ: 2026-03-13)</summary>
        [DefaultValue("2026-03-13")]
        public DateTime Date { get; set; }

        public List<AttendanceRecordDTO> Records { get; set; } = new();
    }

    // Sửa trạng thái + ghi chú điểm danh
    public class UpdateAttendanceDTO
    {
        /// <summary>Present | Absent | Late | Excused | Sick</summary>
        [DefaultValue("Present")]
        public string Status { get; set; } = null!;

        /// <summary>Ghi chú (ví dụ: Sốt 39 độ, đã gọi phụ huynh)</summary>
        [DefaultValue(null)]
        public string? Notes { get; set; }
    }

    // Cập nhật giờ checkout khi bé được đón về
    // CheckOutTime tự động = DateTime.Now, không cần truyền vào
    public class CheckOutDTO
    {
        /// <summary>Ghi chú khi đón về (ví dụ: Ba đón lúc 16:30)</summary>
        [DefaultValue(null)]
        public string? Notes { get; set; }
    }

    // ============================================================
    // FILTER / HISTORY DTOs
    // ============================================================

    public class AttendanceHistoryDTO
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = null!;
        public int TotalDays { get; set; }
        public int PresentDays { get; set; }
        public int AbsentDays { get; set; }
        public int LateDays { get; set; }
        public int SickDays { get; set; }
        public int ExcusedDays { get; set; }
        public double AttendanceRate { get; set; }  // % có mặt
        public List<AttendanceDTO> Records { get; set; } = new();
    }
}