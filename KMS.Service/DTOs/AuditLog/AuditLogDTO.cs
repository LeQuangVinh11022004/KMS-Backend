using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.DTOs.AuditLog
{
    public class AuditLogDTO
    {
        public int LogId { get; set; }
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public string Action { get; set; } = null!;  // Login, Logout, Create, Update, Delete
        public string? TableName { get; set; }
        public int? RecordId { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public string? IPAddress { get; set; }
        public string? UserAgent { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AuditLogFilterDTO
    {
        public int? UserId { get; set; }
        public string? Action { get; set; }
        public string? TableName { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
