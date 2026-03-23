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


        //promt template
        private const string SystemInstructionTemplate =
        "Bạn là trợ lý ảo trường mầm non KMS. Dựa trên dữ liệu: {0}. Hãy trả lời phụ huynh câu hỏi sau một cách thân thiện: {1}";

        public ChatAiService(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _apiKey = config["Gemini:ApiKey"]?.Trim();
        }

        // check xem cau hoi co nam trong domain cua he thong hay 0
        private bool IsOutOfScope(string question)
        {
            string[] allowed =
            {
                "bé","con","điểm danh","lớp",
                "thực đơn","ăn","ngủ","báo cáo",
                "hoạt động","giáo viên"
            };

            return !allowed.Any(k => question.ToLower().Contains(k));
        }

        // tom tat bao cao sinh hoat
        private async Task<string> SummarizeReport(string text)
        {
            var googleAi = new GoogleAI(_apiKey);
            var model = googleAi.GenerativeModel(Model.Gemini25Flash);

            var prompt = $"Tóm tắt báo cáo mầm non sau trong 1 câu ngắn: {text}";

            var response = await model.GenerateContent(prompt);

            return response.Text;
        }

        public async Task<ChatResponseDTO> GetAiResponseAsync(int userId, ChatRequestDTO request)
        {

            // kiem tra neu cau hoi ngoai domain thi tra loi san
            if (IsOutOfScope(request.Prompt))
            {
                return new ChatResponseDTO
                {
                    Answer = "Dạ em chỉ hỗ trợ thông tin liên quan đến bé và hoạt động tại trường KMS thôi ạ.",
                    SentAt = DateTime.Now
                };
            }

            // lay ds hs cua phu huynh
            var students = await (from sp in _context.KmsStudentParents
                                  join s in _context.KmsStudents on sp.StudentId equals s.StudentId
                                  where sp.Parent.UserId == userId
                                  select new
                                  {
                                      s.StudentId,
                                      s.FullName
                                  }).ToListAsync();

            // lay lich su chat gan nhat de lam memory
            var memory = await _context.KmsChatHistories
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.SentAt)
                .Take(5)
                .ToListAsync();

            var historyContext = string.Join("\n",
                memory.Select(x =>
                    $"Parent: {x.UserMessage}\nAI: {x.AiResponse}"
            ));

            var studentIds = students.Select(x => x.StudentId).ToList();

            var today = DateOnly.FromDateTime(DateTime.Today);


            // lay tt diem danh
            var attendances = await _context.KmsAttendances
                .Where(x => studentIds.Contains(x.StudentId)
                    && x.AttendanceDate == today)
                .ToListAsync();

            // lay daily rp da publish
            var reports = await _context.KmsDailyReports
                .Where(x => studentIds.Contains(x.StudentId)
                    && x.ReportDate == today
                    && x.Status == "Published")
                .ToListAsync();

            // lay menu
            var menus = await _context.KmsMenus
                .Where(x => x.MenuDate == today)
                .ToListAsync();

            var contextData = "";


            // build context cho AI
            foreach (var student in students)
            {
                var attendance = attendances
                    .FirstOrDefault(x => x.StudentId == student.StudentId);

                var report = reports
                    .FirstOrDefault(x => x.StudentId == student.StudentId);

                var menuInfo = string.Join(", ",
                    menus.Select(m => $"{m.MealType}: {m.MenuContent}"));

                var attendanceInfo = attendance != null
                    ? attendance.Status
                    : "chưa điểm danh";

                var reportInfo = report != null
                    ? await SummarizeReport(report.LivingData)
                    : "chưa có báo cáo";

                contextData +=
                    $"Bé {student.FullName}: " +
                    $"Điểm danh: {attendanceInfo}. " +
                    $"Thực đơn: {menuInfo}. " +
                    $"Sinh hoạt: {reportInfo}. ";
            }

            //system prompt
            string prompt = $@"
            Bạn là trợ lý ảo trường mầm non KMS.

            CHỈ trả lời các câu hỏi liên quan:
            - bé
            - điểm danh
            - thực đơn
            - sinh hoạt
            - báo cáo

            Nếu ngoài phạm vi hãy trả lời:
            'Dạ em chỉ hỗ trợ thông tin của bé tại trường KMS thôi ạ.'

            Dữ liệu hệ thống:

            {contextData}

            Lịch sử chat:

            {historyContext}

            Câu hỏi phụ huynh:

            {request.Prompt}
            ";

            //call gemini ai
            var googleAi = new GoogleAI(_apiKey);
            var model = googleAi.GenerativeModel(Model.Gemini25Flash);

            var response = await model.GenerateContent(prompt);

            string aiAnswer = response.Text;

            //luu lich su chat
            _context.KmsChatHistories.Add(new KmsChatHistory
            {
                UserId = userId,
                UserMessage = request.Prompt,
                AiResponse = aiAnswer,
                SentAt = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return new ChatResponseDTO
            {
                Answer = aiAnswer,
                SentAt = DateTime.Now
            };
        }
    }
}