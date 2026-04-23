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
                history.AddSystemMessage(@"
                    Bạn là trợ lý ảo thân thiện của trường mầm non KMS.
                    - CHỈ giải đáp các vấn đề về học sinh, thực đơn, điểm danh, sinh hoạt.
                    - Từ chối khéo léo các câu hỏi ngoài lề (ví dụ: thời tiết, chứng khoán...).
                    - Luôn dùng công cụ 'get_my_students' để kiểm tra danh sách con của phụ huynh trước khi tra cứu các thông tin cần StudentId.
                ");

                var oldChats = await _context.KmsChatHistories
                    .Where(x => x.UserId == userId)
                    .OrderByDescending(x => x.SentAt)
                    .Take(3).ToListAsync();

                foreach (var chat in oldChats.OrderBy(x => x.SentAt))
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