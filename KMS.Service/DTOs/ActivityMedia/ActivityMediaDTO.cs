using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.DTOs.ActivityMedia
{
    public class ActivityMediaDTO
    {
        public int MediaId { get; set; }
        public int ActivityId { get; set; }

        public string MediaUrl { get; set; } = null!;

        // Photo | Video
        public string MediaType { get; set; } = "Photo";

        public string? Caption { get; set; }

        public DateTime UploadedAt { get; set; }
    }
    public class ActivityMediaCreateDTO
    {
        public IFormFile File { get; set; } = null!;

        public string MediaType { get; set; } = "Photo";

        public string? Caption { get; set; }
    }
}
