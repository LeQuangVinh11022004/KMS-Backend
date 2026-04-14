using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.DTOs.Notification
{
    public class NotificationDTO
    {
        public int NotificationId { get; set; }

        public int UserId { get; set; }
        public int? StudentId { get; set; }
        public string Type { get; set; } = null!;

        public string Title { get; set; } = null!;

        public string Message { get; set; } = null!;

        public string? RelatedEntityType { get; set; }

        public int? RelatedEntityId { get; set; }

        public bool IsRead { get; set; }

        public DateTime? ReadAt { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
    public class CreateNotificationDTO
    {
        public int UserId { get; set; }
        public string Type { get; set; } = null!;
        public int? StudentId { get; set; }
        public string Title { get; set; } = null!;

        public string Message { get; set; } = null!;

        public string? RelatedEntityType { get; set; }

        public int? RelatedEntityId { get; set; }
    }
    public class UpdateNotificationDTO
    {
        public string Title { get; set; } = null!;

        public string Message { get; set; } = null!;
    }
}
