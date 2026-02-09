using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.DTOs
{
    public class CreateStudentDTO
    {
        [Required] public string StudentCode { get; set; } = string.Empty;
        [Required] public string FullName { get; set; } = string.Empty;
        [Required] public DateOnly DateOfBirth { get; set; }
        [Required] public string Gender { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? Photo { get; set; }
        public string? BloodType { get; set; }
        public string? Allergies { get; set; }
        public string? MedicalNotes { get; set; }
        public int? CreatedBy { get; set; }
    }

    public class UpdateStudentDTO
    {
        public string? FullName { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public string? Photo { get; set; }
        public string? BloodType { get; set; }
        public string? Allergies { get; set; }
        public string? MedicalNotes { get; set; }
        public bool? IsActive { get; set; }
        public int? UpdatedBy { get; set; }
    }

    public class StudentDTO
    {
        public int StudentId { get; set; }
        public string StudentCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? Photo { get; set; }
        public string? BloodType { get; set; }
        public string? Allergies { get; set; }
        public string? MedicalNotes { get; set; }
        public DateOnly EnrollmentDate { get; set; }
        public bool IsActive { get; set; }
        public List<ParentDTO>? Parents { get; set; }
        public string? CurrentClass { get; set; }
    }
}
