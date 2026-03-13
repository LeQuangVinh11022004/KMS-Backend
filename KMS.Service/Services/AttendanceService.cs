using KMS.Repository.Entities;
using KMS.Repository.Interfaces;
using KMS.Service.DTOs;
using KMS.Service.DTOs.Attendance;
using KMS.Service.DTOs.Role;
using KMS.Service.Interfaces;

namespace KMS.Service.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IClassRepository _classRepository;

        private static readonly HashSet<string> ValidStatuses =
            new() { "Present", "Absent", "Late", "Excused", "Sick" };

        public AttendanceService(
            IAttendanceRepository attendanceRepository,
            IClassRepository classRepository)
        {
            _attendanceRepository = attendanceRepository;
            _classRepository = classRepository;
        }

        // ============================================================
        // Teacher mở danh sách lớp theo ngày
        // Trả về toàn bộ học sinh + trạng thái đã điểm danh (nếu có)
        // ============================================================

        public async Task<BaseResponseDTO> GetClassAttendanceSheetAsync(int classId, DateTime date)
        {
            try
            {
                var klass = await _classRepository.GetByIdAsync(classId);
                if (klass == null)
                    return new BaseResponseDTO { Success = false, Message = "Class not found" };

                var dateOnly = DateOnly.FromDateTime(date);

                // Lấy tất cả học sinh trong lớp
                var students = await _attendanceRepository.GetStudentsByClassAsync(classId);

                // Lấy attendance records đã có trong ngày
                var existingRecords = await _attendanceRepository.GetByClassAndDateAsync(classId, dateOnly);
                var recordMap = existingRecords.ToDictionary(a => a.StudentId);

                var rows = students.Select(s =>
                {
                    recordMap.TryGetValue(s.StudentId, out var record);
                    return new StudentAttendanceRowDTO
                    {
                        StudentId = s.StudentId,
                        StudentName = s.FullName,
                        StudentCode = s.StudentCode,
                        StudentPhoto = s.Photo,
                        Gender = s.Gender,
                        AttendanceId = record?.AttendanceId,
                        Status = record?.Status,
                        CheckInTime = record?.CheckInTime,
                        CheckOutTime = record?.CheckOutTime,
                        Notes = record?.Notes
                    };
                }).ToList();

                var sheet = new ClassAttendanceSheetDTO
                {
                    ClassId = classId,
                    ClassName = klass.ClassName,
                    Date = date,
                    IsSubmitted = existingRecords.Any(),
                    TotalStudents = rows.Count,
                    PresentCount = rows.Count(r => r.Status == "Present"),
                    AbsentCount = rows.Count(r => r.Status == "Absent"),
                    LateCount = rows.Count(r => r.Status == "Late"),
                    SickCount = rows.Count(r => r.Status == "Sick"),
                    ExcusedCount = rows.Count(r => r.Status == "Excused"),
                    Students = rows
                };

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = sheet.IsSubmitted
                        ? $"Lớp {klass.ClassName} đã điểm danh ngày {date:dd/MM/yyyy}"
                        : $"Lớp {klass.ClassName} chưa điểm danh ngày {date:dd/MM/yyyy}",
                    Data = sheet
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        // ============================================================
        // Lưu điểm danh cả lớp 1 lần (bulk)
        // Nếu học sinh đã có record → update, chưa có → create
        // ============================================================

        public async Task<BaseResponseDTO> SubmitBulkAttendanceAsync(
            BulkAttendanceDTO dto, int takenByTeacherId)
        {
            try
            {
                if (dto.Records == null || !dto.Records.Any())
                    return new BaseResponseDTO { Success = false, Message = "Không có dữ liệu điểm danh" };

                // Validate status
                var invalidStatuses = dto.Records
                    .Where(r => !ValidStatuses.Contains(r.Status))
                    .Select(r => r.Status)
                    .Distinct().ToList();

                if (invalidStatuses.Any())
                    return new BaseResponseDTO
                    {
                        Success = false,
                        Message = $"Status không hợp lệ: {string.Join(", ", invalidStatuses)}. Chỉ chấp nhận: Present, Absent, Late, Excused, Sick"
                    };

                var dateOnly = DateOnly.FromDateTime(dto.Date);
                var toCreate = new List<KmsAttendance>();
                var updated = 0;

                foreach (var record in dto.Records)
                {
                    var existing = await _attendanceRepository.GetByStudentAndDateAsync(record.StudentId, dateOnly);

                    if (existing != null)
                    {
                        // Update record đã có
                        existing.Status = record.Status;
                        // Giữ CheckInTime gốc, không ghi đè
                        // CheckOutTime không thay đổi ở đây
                        existing.Notes = record.Notes;
                        existing.UpdatedBy = takenByTeacherId;
                        existing.UpdatedAt = DateTime.Now;
                        await _attendanceRepository.UpdateAsync(existing);
                        updated++;
                    }
                    else
                    {
                        // Thêm mới
                        toCreate.Add(new KmsAttendance
                        {
                            StudentId = record.StudentId,
                            ClassId = dto.ClassId,
                            AttendanceDate = dateOnly,
                            Status = record.Status,
                            CheckInTime = TimeOnly.FromDateTime(DateTime.Now), // tự set giờ hiện tại
                            CheckOutTime = null, // chưa có lúc điểm danh, cập nhật sau
                            Notes = record.Notes,
                            TakenBy = takenByTeacherId,
                            TakenAt = DateTime.Now
                        });
                    }
                }

                if (toCreate.Any())
                    await _attendanceRepository.CreateBulkAsync(toCreate);

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"Điểm danh thành công: {toCreate.Count} mới, {updated} cập nhật",
                    Data = new
                    {
                        Date = dto.Date.ToString("dd/MM/yyyy"),
                        Created = toCreate.Count,
                        Updated = updated,
                        Total = toCreate.Count + updated
                    }
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        // ============================================================
        // Sửa điểm danh 1 học sinh
        // ============================================================

        public async Task<BaseResponseDTO> UpdateAttendanceAsync(
            int attendanceId, UpdateAttendanceDTO dto, int updatedByTeacherId)
        {
            try
            {
                if (!ValidStatuses.Contains(dto.Status))
                    return new BaseResponseDTO
                    {
                        Success = false,
                        Message = $"Status không hợp lệ. Chỉ chấp nhận: {string.Join(", ", ValidStatuses)}"
                    };

                var attendance = await _attendanceRepository.GetByIdAsync(attendanceId);
                if (attendance == null)
                    return new BaseResponseDTO { Success = false, Message = "Attendance record not found" };

                attendance.Status = dto.Status;
                attendance.Notes = dto.Notes;
                // CheckInTime và CheckOutTime không thay đổi ở đây
                attendance.UpdatedBy = updatedByTeacherId;
                attendance.UpdatedAt = DateTime.Now;

                await _attendanceRepository.UpdateAsync(attendance);

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = "Cập nhật điểm danh thành công",
                    Data = MapToDTO(attendance)
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        // ============================================================
        // Lịch sử điểm danh của 1 học sinh
        // ============================================================

        public async Task<BaseResponseDTO> GetStudentHistoryAsync(
            int studentId, DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                var fromDateOnly = fromDate.HasValue ? DateOnly.FromDateTime(fromDate.Value) : (DateOnly?)null;
                var toDateOnly = toDate.HasValue ? DateOnly.FromDateTime(toDate.Value) : (DateOnly?)null;

                var records = await _attendanceRepository.GetByStudentAsync(studentId, fromDateOnly, toDateOnly);
                var list = records.ToList();

                if (!list.Any())
                    return new BaseResponseDTO
                    {
                        Success = true,
                        Message = "Không có dữ liệu điểm danh",
                        Data = new AttendanceHistoryDTO { StudentId = studentId }
                    };

                var studentName = list.First().Student?.FullName ?? "";

                var history = new AttendanceHistoryDTO
                {
                    StudentId = studentId,
                    StudentName = studentName,
                    TotalDays = list.Count,
                    PresentDays = list.Count(r => r.Status == "Present"),
                    AbsentDays = list.Count(r => r.Status == "Absent"),
                    LateDays = list.Count(r => r.Status == "Late"),
                    SickDays = list.Count(r => r.Status == "Sick"),
                    ExcusedDays = list.Count(r => r.Status == "Excused"),
                    AttendanceRate = list.Count > 0
                        ? Math.Round((double)list.Count(r => r.Status is "Present" or "Late") / list.Count * 100, 1)
                        : 0,
                    Records = list.Select(MapToDTO).ToList()
                };

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"Lịch sử điểm danh của {studentName}: {list.Count} ngày",
                    Data = history
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        // ============================================================
        // CHECKOUT - Cập nhật giờ ra về khi bé được đón
        // ============================================================

        public async Task<BaseResponseDTO> CheckOutAsync(
            int attendanceId, CheckOutDTO dto, int updatedByTeacherId)
        {
            try
            {
                var attendance = await _attendanceRepository.GetByIdAsync(attendanceId);
                if (attendance == null)
                    return new BaseResponseDTO { Success = false, Message = "Attendance record not found" };

                // Luôn set giờ thực tế lúc checkout
                attendance.CheckOutTime = TimeOnly.FromDateTime(DateTime.Now);

                if (!string.IsNullOrEmpty(dto.Notes))
                    attendance.Notes = dto.Notes;

                attendance.UpdatedBy = updatedByTeacherId;
                attendance.UpdatedAt = DateTime.Now;

                await _attendanceRepository.UpdateAsync(attendance);

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"Đã cập nhật giờ ra về: {attendance.CheckOutTime}",
                    Data = MapToDTO(attendance)
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        // ============================================================
        // HELPERS
        // ============================================================

        private static TimeOnly? ParseTime(string? timeStr)
        {
            if (string.IsNullOrWhiteSpace(timeStr)) return null;
            if (TimeOnly.TryParse(timeStr, out var result)) return result;
            return null;
        }

        // ============================================================
        // MAPPER
        // ============================================================

        private AttendanceDTO MapToDTO(KmsAttendance a) => new()
        {
            AttendanceId = a.AttendanceId,
            StudentId = a.StudentId,
            StudentName = a.Student?.FullName ?? "",
            StudentCode = a.Student?.StudentCode,
            StudentPhoto = a.Student?.Photo,
            ClassId = a.ClassId,
            ClassName = a.Class?.ClassName ?? "",
            AttendanceDate = a.AttendanceDate.ToDateTime(TimeOnly.MinValue),
            Status = a.Status,
            CheckInTime = a.CheckInTime,
            CheckOutTime = a.CheckOutTime,
            Notes = a.Notes,
            TakenByName = a.TakenByNavigation?.User?.FullName,
            TakenAt = a.TakenAt,
            UpdatedByName = a.UpdatedByNavigation?.User?.FullName,
            UpdatedAt = a.UpdatedAt
        };
    }
}