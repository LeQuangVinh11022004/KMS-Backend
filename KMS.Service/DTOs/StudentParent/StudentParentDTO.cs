using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.DTOs.StudentParent
{
    public class StudentParentDTO
    {
        public int StudentId { get; set; }
        public string StudentCode { get; set; } = null!;
        public string StudentName { get; set; } = null!;
        public int ParentId { get; set; }
        public string ParentName { get; set; } = null!;
        public string? ParentEmail { get; set; }
        public string? ParentPhone { get; set; }
        public string Relationship { get; set; } = null!; // Father, Mother, Guardian, Other
        public bool IsPrimaryContact { get; set; }
        public DateTime AssignedAt { get; set; }
    }

    public class LinkStudentParentDTO
    {
        public int ParentId { get; set; }
        public string Relationship { get; set; } = null!; // Father, Mother, Guardian, Other
        public bool IsPrimaryContact { get; set; } = false;
    }

    public class UpdateStudentParentDTO
    {
        public string? Relationship { get; set; }
        public bool? IsPrimaryContact { get; set; }
    }
}
