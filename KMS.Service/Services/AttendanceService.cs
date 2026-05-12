using KMS.Repository.Entities;
using KMS.Repository.Interfaces;
using KMS.Service.DTOs;
using KMS.Service.DTOs.Attendance;
using KMS.Service.DTOs.Notification;
using KMS.Service.DTOs.Role;
using KMS.Service.Interfaces;

namespace KMS.Service.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IClassRepository _classRepository;
        private readonly IParentRepository _parentRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IEmailService _emailService;
        private readonly INotificationHubService _notificationHub;

        private static readonly HashSet<string> ValidStatuses =
            new() { "Present", "Absent", "Late", "Excused", "Sick" };

        public AttendanceService(
            IAttendanceRepository attendanceRepository,
            IClassRepository classRepository,
            IParentRepository parentRepository,
            INotificationRepository notificationRepository,
            IEmailService emailService,
            INotificationHubService notificationHub)
        {
            _attendanceRepository = attendanceRepository;
            _classRepository = classRepository;
            _parentRepository = parentRepository;
            _notificationRepository = notificationRepository;
            _emailService = emailService;
            _notificationHub = notificationHub;
        }

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
                    IsSubmitted = rows.Count > 0 && rows.All(r => r.Status != null),
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

                var studentIds = dto.Records.Select(r => r.StudentId).ToList();


                var existingAttendances =
                    await _attendanceRepository.GetByClassAndDateForStudentsAsync(
                        dto.ClassId,
                        dateOnly,
                        studentIds);

                var attendanceMap = existingAttendances.ToDictionary(a => a.StudentId);


                var parentLinks = await _parentRepository.GetParentsByStudentIdsAsync(studentIds);

                var parentMap = parentLinks
                    .GroupBy(p => p.StudentId)
                    .ToDictionary(g => g.Key, g => g.ToList());

                var notifications = new List<KmsNotification>();



                foreach (var record in dto.Records)
                {
                    attendanceMap.TryGetValue(record.StudentId, out var existing);

                    if (existing != null)
                    {
                        existing.Status = record.Status;
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
                            CheckInTime = TimeOnly.FromDateTime(DateTime.Now), 
                            CheckOutTime = null, 
                            Notes = record.Notes,
                            TakenBy = takenByTeacherId,
                            TakenAt = DateTime.Now
                        });
                    }
                    await SendAttendanceNotification(record.StudentId, record.Status);
                    // notification

                }
                if (toCreate.Any())
                    await _attendanceRepository.CreateBulkAsync(toCreate);

                if (notifications.Any())
                    await _notificationRepository.CreateBulkAsync(notifications);


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
                attendance.UpdatedBy = updatedByTeacherId;
                attendance.UpdatedAt = DateTime.Now;

                await _attendanceRepository.UpdateAsync(attendance);

                await SendAttendanceNotification(attendance.StudentId, dto.Status);
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

        public async Task<BaseResponseDTO> CheckOutAsync(
            int attendanceId, CheckOutDTO dto, int updatedByTeacherId)
        {
            try
            {
                var attendance = await _attendanceRepository.GetByIdAsync(attendanceId);
                if (attendance == null)
                    return new BaseResponseDTO { Success = false, Message = "Attendance record not found" };

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
        private async Task SendAttendanceNotification(int studentId, string status)
        {
            if (status != "Absent" && status != "Late")
                return;

            var parents = await _parentRepository.GetParentsByStudentIdAsync(studentId);

            if (!parents.Any())
                return;

            var notifications = new List<KmsNotification>();

            foreach (var parent in parents)
            {
                notifications.Add(new KmsNotification
                {
                    UserId = parent.UserId,
                    StudentId = studentId,
                    Type = "Attendance",
                    Title = "Thông báo điểm danh",
                    Message = status == "Absent"
                        ? "Con của bạn hôm nay vắng mặt."
                        : "Con của bạn hôm nay đến lớp trễ.",
                    RelatedEntityType = "Attendance",
                    RelatedEntityId = studentId,
                    IsRead = false,
                    CreatedAt = DateTime.Now
                });

                if (!string.IsNullOrEmpty(parent.User?.Email))
                {
                    var studentName = parent.KmsStudentParents
                    .FirstOrDefault()?.Student?.FullName ?? "Học sinh";

                    await _emailService.SendAttendanceAlertEmailAsync(
                        parent.User.Email,
                        parent.User.FullName,
                        studentName,
                        status,
                        DateTime.Now
                    );
                }
            }

            await _notificationRepository.CreateBulkAsync(notifications);

            foreach (var n in notifications)
            {
                await _notificationHub.SendNotificationToUserAsync(n.UserId, new NotificationDTO
                {
                    NotificationId = n.NotificationId,
                    UserId = n.UserId,
                    StudentId = n.StudentId,
                    Type = n.Type,
                    Title = n.Title,
                    Message = n.Message,
                    RelatedEntityType = n.RelatedEntityType,
                    RelatedEntityId = n.RelatedEntityId,
                    IsRead = false,
                    CreatedAt = n.CreatedAt
                });
            }
        }

        private static TimeOnly? ParseTime(string? timeStr)
        {
            if (string.IsNullOrWhiteSpace(timeStr)) return null;
            if (TimeOnly.TryParse(timeStr, out var result)) return result;
            return null;
        }

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