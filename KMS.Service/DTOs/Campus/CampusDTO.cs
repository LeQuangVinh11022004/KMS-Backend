using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.DTOs.Campus
{
    public class CampusDTO
    {
        public int CampusId { get; set; }
        public string CampusName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public bool? IsActive { get; set; }
    }
}
