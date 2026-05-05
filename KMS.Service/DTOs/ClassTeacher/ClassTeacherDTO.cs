using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.DTOs.ClassTeacher
{
    public class ClassTeacherDTO
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; } = null!;
        public int TeacherId { get; set; }
        public string? TeacherCode { get; set; }
        public string TeacherName { get; set; } = null!;
        public string? Email { get; set; }
        public string? Specialization { get; set; }
        public DateTime AssignedAt { get; set; }
        public string? AssignedByName { get; set; }
    }

    public class AssignTeacherDTO
    {
        public int TeacherId { get; set; }
    }

}
