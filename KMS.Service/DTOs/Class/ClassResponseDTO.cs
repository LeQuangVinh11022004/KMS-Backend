using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.DTOs.Class
{
    public class ClassResponseDTO
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; } = null!;
        public int SchoolYearId { get; set; }
        public string? AgeGroup { get; set; }
        public int? MaxCapacity { get; set; }
        public int? CurrentEnrollment { get; set; }
        public string? Room { get; set; }
        public bool? IsActive { get; set; }
    }
}
