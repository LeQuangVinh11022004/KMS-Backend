using KMS.Repository.Entities;
using KMS.Service.DTOs.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.Interfaces
{
    public interface IChatAiService
    {
        Task<ChatResponseDTO> GetAiResponseAsync(int userId, ChatRequestDTO request);
        Task<List<KmsChatHistory>> GetUserChatHistoryAsync(int userId);
    }
}
