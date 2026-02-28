using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.DTOs.ClassActivity
{
    public class ClassActivityDTO
    {
        public int ActivityId { get; set; }
        public int ClassId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateOnly? ActivityDate { get; set; }
    }
}
