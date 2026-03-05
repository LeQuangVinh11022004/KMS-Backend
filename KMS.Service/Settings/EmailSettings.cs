using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.Settings
{
    public class EmailSettings
    {
        public string SmtpHost { get; set; } = "smtp.gmail.com";
        public int SmtpPort { get; set; } = 587;
        public string SenderEmail { get; set; } = null!;
        public string SenderName { get; set; } = null!;
        public string AppPassword { get; set; } = null!;
    }
}
