using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.DTOs.Chat
{
    public class ChatRequestDTO
    {
        public string Prompt { get; set; }
    }

    public class ChatResponseDTO
    {
        public string Answer { get; set; }
        public DateTime SentAt { get; set; }
    }
}
