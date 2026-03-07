using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.DTOs.KitchenSample
{
    public class KitchenSampleDTO
    {
        public int? MenuId { get; set; }

        public DateOnly? SampleDate { get; set; }

        public TimeOnly? SampleTime { get; set; }

        public DateTime? DisposalTime { get; set; }

        public string Note { get; set; }
    }
}
