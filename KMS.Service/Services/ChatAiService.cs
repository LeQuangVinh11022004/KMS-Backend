    using KMS.Repository.Entities;
    using KMS.Service.DTOs.Chat;
    using KMS.Service.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Mscc.GenerativeAI;
    using Mscc.GenerativeAI.Types;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    namespace KMS.Service.Services
    {
        public class ChatAiService : IChatAiService
        {
            private readonly ApplicationDbContext _context;
            private readonly string _apiKey;
            private readonly string _modelName;

            private const string SystemInstructionTemplate =
            "Bạn là trợ lý ảo trường mầm non KMS. Dựa trên dữ liệu: {0}. Hãy trả lời phụ huynh câu hỏi sau một cách thân thiện: {1}";

            public ChatAiService(ApplicationDbContext context, IConfiguration config)
            {
                _context = context;
                _apiKey = config["Gemini:ApiKey"]?.Trim();
                _modelName = config["Gemini:Model"]?.Trim() ?? "gemini-1.5-flash";
            }

            public async Task<ChatResponseDTO> GetAiResponseAsync(int userId, ChatRequestDTO request)
            {
            var today = DateOnly.FromDateTime(DateTime.Today);

            //lay thong tin hoc sinh va lop cua phu huynh
            var data = await (from sp in _context.KmsStudentParents
                              join s in _context.KmsStudents on sp.StudentId equals s.StudentId
                              join cs in _context.KmsClassStudents on s.StudentId equals cs.StudentId
                              join c in _context.KmsClasses on cs.ClassId equals c.ClassId
                              where sp.Parent.UserId == userId
                              select new
                              {
                                  StudentId = s.StudentId,
                                  FullName = s.FullName,
                                  ClassName = c.ClassName,
                                  ClassId = c.ClassId,
                                  // lay diem danh hom nay 
                                  Attendance = _context.KmsAttendances
                                      .FirstOrDefault(a => a.StudentId == s.StudentId && a.AttendanceDate == today),
                                  //lay bcao sinh hoat hom nay
                                  Report = _context.KmsDailyReports
                                      .FirstOrDefault(r => r.StudentId == s.StudentId && r.ReportDate == today && r.Status == "Published"),
                                  //lay menu hom nay cua lop
                                  Menu = _context.KmsMenus
                                      .Where(m => (m.ClassId == c.ClassId || m.ClassId == null) && m.MenuDate == today)
                                      .ToList()
                              }).FirstOrDefaultAsync();

            if (data == null)
                return new ChatResponseDTO { Answer = "Dạ, hiện tại em chưa tìm thấy thông tin của bé nhà mình trên hệ thống ạ.", SentAt = DateTime.Now };

            // xay dung context
            var menuInfo = data.Menu.Any()
                ? string.Join(", ", data.Menu.Select(m => $"{m.MealType}: {m.MenuContent}"))
                : "Chưa cập nhật thực đơn";

            var attendanceInfo = data.Attendance != null
                ? (data.Attendance.Status == "Present" ? $"đã đến lớp lúc {data.Attendance.CheckInTime}" : $"vắng mặt ({data.Attendance.Status})")
                : "chưa có dữ liệu điểm danh";

            string contextData = $"- Bé: {data.FullName}, Lớp: {data.ClassName}. " +
                                 $"- Điểm danh hôm nay: {attendanceInfo}. " +
                                 $"- Thực đơn hôm nay: {menuInfo}. " +
                                 $"- Sinh hoạt: {data.Report?.LivingData ?? "Giáo viên chưa cập nhật cụ thể"}. " +
                                 $"- Nhận xét: {data.Report?.EducationalData ?? "Đang đợi cô giáo ghi chú thêm"}.";

            //system prompt
            string systemInstruction = "Bạn là Cô Trợ Lý Ảo thân thiện của trường mầm non KMS. " +
                "Cách xưng hô: Gọi phụ huynh là 'Ba/Mẹ', xưng là 'Dạ em' hoặc 'KMS'. " +
                "Luôn dùng từ ngữ nhẹ nhàng, yêu thương vì đây là môi trường giáo dục mầm non. " +
                $"Dữ liệu thực tế hôm nay: {contextData}. " +
                $"Hãy trả lời câu hỏi này của Ba/Mẹ: {request.Prompt}";

            //call gemini ai
            var googleAi = new GoogleAI(_apiKey);
            var model = googleAi.GenerativeModel(Model.Gemini25Flash);
                string fullPrompt = string.Format(SystemInstructionTemplate, contextData, request.Prompt);

                var response = await model.GenerateContent(fullPrompt);
                string aiAnswer = response.Text;

                //luu lich su chat
                var history = new KmsChatHistory
                {
                    UserId = userId,
                    StudentId = data?.StudentId,
                    UserMessage = request.Prompt,
                    AiResponse = aiAnswer,
                    SentAt = DateTime.Now
                };
                _context.KmsChatHistories.Add(history);
                await _context.SaveChangesAsync();

                return new ChatResponseDTO { Answer = aiAnswer, SentAt = DateTime.Now };
            }
        }
    }