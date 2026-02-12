using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.DTOs.Semester
{
    public class SemesterDTO
    {
        public int SemesterId { get; set; }
        public int SchoolYearId { get; set; }
        public string SemesterName { get; set; } = null!;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public bool? IsActive { get; set; }
    }
}
