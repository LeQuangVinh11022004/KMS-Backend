using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.DTOs.Announcement
{
    public class AnnouncementDTO
    {
        public int AnnouncementId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string TargetAudience { get; set; }
        public int? TargetClassId { get; set; }
        public string Priority { get; set; }
        public bool? IsPublished { get; set; }
        public DateTime? PublishedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}
