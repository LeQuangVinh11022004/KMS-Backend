using KMS.Service.DTOs;
using KMS.Service.DTOs.Attendance;
using KMS.Service.DTOs.Role;

namespace KMS.Service.Interfaces
{
    public interface IAttendanceService
    {
        // Teacher mở danh sách lớp theo ngày
        Task<BaseResponseDTO> GetClassAttendanceSheetAsync(int classId, DateTime date);

        // Lưu điểm danh cả lớp 1 lần
        Task<BaseResponseDTO> SubmitBulkAttendanceAsync(BulkAttendanceDTO dto, int takenByTeacherId);

        // Sửa trạng thái + ghi chú điểm danh
        Task<BaseResponseDTO> UpdateAttendanceAsync(int attendanceId, UpdateAttendanceDTO dto, int updatedByTeacherId);

        // Cập nhật giờ checkout khi bé được đón về
        Task<BaseResponseDTO> CheckOutAsync(int attendanceId, CheckOutDTO dto, int updatedByTeacherId);

        // Lịch sử điểm danh của 1 học sinh (Parent + Teacher xem)
        Task<BaseResponseDTO> GetStudentHistoryAsync(int studentId, DateTime? fromDate, DateTime? toDate);
    }
}