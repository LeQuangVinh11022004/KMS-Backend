using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.DTOs.ActivityPhoto
{
    public class ActivityPhotoDTO
    {
        public int PhotoId { get; set; }
        public int ActivityId { get; set; }
        public string PhotoUrl { get; set; }
        public string Caption { get; set; }
    }
}
