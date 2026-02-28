using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.DTOs.Teacher
{
    public class CreateTeacherDTO
    {
        [Required] public int UserId { get; set; }
        [Required] public string TeacherCode { get; set; } = string.Empty;
        public string? Specialization { get; set; }
        public DateOnly? HireDate { get; set; }
        public decimal? Salary { get; set; }
    }

    public class UpdateTeacherDTO
    {
        public string? Specialization { get; set; }
        public DateOnly? HireDate { get; set; }
        public decimal? Salary { get; set; }
        public bool? IsActive { get; set; }
    }

    public class TeacherDTO
    {
        public int TeacherId { get; set; }
        public int UserId { get; set; }
        public string TeacherCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Specialization { get; set; }
        public DateOnly? HireDate { get; set; }
        public decimal? Salary { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
