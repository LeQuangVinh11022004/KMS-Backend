using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.DTOs.ClassStudent
{
    public class ClassStudentDTO
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; } = null!;
        public int StudentId { get; set; }
        public string StudentCode { get; set; } = null!;
        public string StudentName { get; set; } = null!;
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public DateTime EnrolledAt { get; set; }
        public string? EnrolledByName { get; set; }
    }

    public class EnrollStudentDTO
    {
        public int StudentId { get; set; }
    }

    public class EnrollMultipleStudentsDTO
    {
        public List<int> StudentIds { get; set; } = new();
    }
}
