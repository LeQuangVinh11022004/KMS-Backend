using KMS.Service.DTOs.Teacher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.DTOs.Profile
{
    public class ProfileDTO
    {
        public int UserId { get; set; }
        public string Username { get; set; } = "";
        public string FullName { get; set; } = "";
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Avatar { get; set; }
        public bool IsActive { get; set; }
        public List<string> Roles { get; set; } = new();

        // Teacher
        public int? TeacherId { get; set; }
        public List<AssignedClassDTO>? AssignedClasses { get; set; }

        // Parent
        public int? ParentId { get; set; }
        public List<ChildSummaryDTO>? Children { get; set; }
    }

    public class ChildSummaryDTO
    {
        public int StudentId { get; set; }
        public string FullName { get; set; } = "";
        public int? ClassId { get; set; }
        public string? ClassName { get; set; }
    }
}
