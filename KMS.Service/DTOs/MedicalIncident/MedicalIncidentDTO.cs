using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.DTOs.MedicalIncident
{
    public class MedicalIncidentDTO
    {
        public int StudentId { get; set; }
        public DateTime? IncidentDate { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string ActionTaken { get; set; }
        public string Status { get; set; }
    }
}
