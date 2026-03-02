using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.DTOs.HealthCheck
{
    public class HealthCheckDTO
    {
        public int StudentId { get; set; }
        public DateOnly? CheckDate { get; set; }
        public decimal? Height { get; set; }
        public decimal? Weight { get; set; }
        public string EyeSight { get; set; }
        public string DentalStatus { get; set; }
        public string Note { get; set; }
    }
}
