using KMS.Repository.Entities;
using KMS.Service.DTOs.Chat;
using KMS.Service.Interfaces;
using KMS.Service.Plugins;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;

namespace KMS.Service.Services
{
    public class ChatAiService : IChatAiService
    {
        private readonly ApplicationDbContext _context;
        private readonly string _apiKey;

        public ChatAiService(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _apiKey = config["Gemini:ApiKey"]?.Trim() ?? string.Empty;
        }
        public async Task<List<KmsChatHistory>> GetUserChatHistoryAsync(int userId)
        {
            return await _context.KmsChatHistories
                .Where(x => x.UserId == userId)
                .OrderBy(x => x.SentAt)
                .ToListAsync();
        }
        public async Task<ChatResponseDTO> GetAiResponseAsync(int userId, ChatRequestDTO request)
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                return new ChatResponseDTO { Answer = "Lỗi: Thiếu API Key", SentAt = DateTime.Now };
            }

            try
            {
                var builder = Kernel.CreateBuilder();
                builder.AddGoogleAIGeminiChatCompletion(
                    modelId: "gemini-2.5-flash",
                    apiKey: _apiKey,
                    apiVersion: GoogleAIVersion.V1_Beta
                );

                builder.Plugins.AddFromObject(new StudentPlugin(_context, userId), "StudentData");
                builder.Plugins.AddFromObject(new MenuPlugin(_context), "MenuData");
                builder.Plugins.AddFromObject(new AttendancePlugin(_context), "AttendanceData");
                builder.Plugins.AddFromObject(new DailyReportPlugin(_context), "ReportData");

                var kernel = builder.Build();
                var chatCompletion = kernel.GetRequiredService<IChatCompletionService>();

                var history = new ChatHistory();
                history.AddSystemMessage($@"
                Bạn là trợ lý ảo AI thông minh và tận tâm của trường mầm non KMS. 
                Nhiệm vụ của bạn là hỗ trợ phụ huynh tra cứu thông tin học tập, sinh hoạt của con em họ.

                Quy tắc hoạt động:
                1. LUÔN GIAO TIẾP TỰ NHIÊN: Khi nhận được dữ liệu (JSON) từ các công cụ (plugins), hãy chuyển hóa nó thành câu trả lời tự nhiên, thân thiện. Không bao giờ hiển thị mã nguồn hoặc chuỗi JSON cho người dùng.
                2. THẤU HIỂU NGỮ CẢNH: Dựa vào lịch sử chat, nếu phụ huynh đang hỏi tiếp về một bé (đã biết StudentId từ câu trước), hãy tự động dùng lại StudentId đó mà không bắt họ phải chọn lại.
                3. Nếu không có dữ liệu, hãy xin lỗi nhẹ nhàng và gợi ý họ liên hệ giáo viên phụ trách.
                4. KHÔNG trả lời các chủ đề ngoài lề (chính trị, thời tiết, giải trí...). Nhẹ nhàng lái câu chuyện về lại trường mầm non KMS.

                Thời gian hiện tại của hệ thống: {DateTime.Now:dd/MM/yyyy HH:mm}.
                ");

                var recentChats = await _context.KmsChatHistories
                    .Where(x => x.UserId == userId)
                    .OrderByDescending(x => x.SentAt)
                    .Take(10)
                    .ToListAsync();

                foreach (var chat in recentChats.OrderBy(x => x.SentAt))
                {
                    history.AddUserMessage(chat.UserMessage);
                    history.AddAssistantMessage(chat.AiResponse);
                }

                history.AddUserMessage(request.Prompt);

                var executionSettings = new GeminiPromptExecutionSettings
                {
                    ToolCallBehavior = GeminiToolCallBehavior.AutoInvokeKernelFunctions
                };

                var response = await chatCompletion.GetChatMessageContentAsync(history, executionSettings, kernel);
                string aiAnswer = response.Content;

                var newHistory = new KmsChatHistory
                {
                    UserId = userId,
                    UserMessage = request.Prompt,
                    AiResponse = aiAnswer,
                    SentAt = DateTime.Now
                };
                _context.KmsChatHistories.Add(newHistory);
                await _context.SaveChangesAsync();

                return new ChatResponseDTO
                {
                    Answer = aiAnswer,
                    SentAt = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                return new ChatResponseDTO
                {
                    Answer = $"Lỗi xử lý AI: {ex.Message}",
                    SentAt = DateTime.Now
                };
            }
        }
    }
}