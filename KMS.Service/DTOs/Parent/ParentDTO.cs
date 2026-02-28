using KMS.Service.DTOs.Student;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.DTOs.Parent
{
    public class CreateParentDTO
    {
        [Required] public int UserId { get; set; }
        public string? Occupation { get; set; }
        public string? WorkAddress { get; set; }
        public string? EmergencyContact { get; set; }
    }

    public class UpdateParentDTO
    {
        public string? Occupation { get; set; }
        public string? WorkAddress { get; set; }
        public string? EmergencyContact { get; set; }
    }

    public class ParentDTO
    {
        public int ParentId { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Occupation { get; set; }
        public string? WorkAddress { get; set; }
        public string? EmergencyContact { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<StudentDTO>? Children { get; set; }
    }

    public class AssignStudentToParentDTO
    {
        [Required] public int StudentId { get; set; }
        [Required] public int ParentId { get; set; }
        [Required] public string Relationship { get; set; } = "Parent";
        public bool IsPrimaryContact { get; set; } = false;
    }
}
