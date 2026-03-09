using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.DTOs.SystemSetting
{
    public class SystemSettingDTO
    {
        public int SettingId { get; set; }
        public string SettingKey { get; set; } = null!;
        public string? SettingValue { get; set; }
        public string? Description { get; set; }
        public string? UpdatedByName { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class UpdateSettingDTO
    {
        public string? SettingValue { get; set; }
    }

    public class UpdateMultipleSettingsDTO
    {
        public Dictionary<string, string?> Settings { get; set; } = new();
    }
}
