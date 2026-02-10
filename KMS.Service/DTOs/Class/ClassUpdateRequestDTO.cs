using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.DTOs.Class
{
    public class ClassUpdateRequestDTO
    {
        [Required]
        public string ClassName { get; set; } = null!;

        public string? AgeGroup { get; set; }

        public int? MaxCapacity { get; set; }

        public string? Room { get; set; }

        public bool? IsActive { get; set; }
    }
}
