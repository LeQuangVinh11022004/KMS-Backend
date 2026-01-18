using System;
using System.Collections.Generic;

namespace KMS.Repository.Entities;

public partial class KmsSystemSetting
{
    public int SettingId { get; set; }

    public string SettingKey { get; set; } = null!;

    public string? SettingValue { get; set; }

    public string? Description { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual KmsUser? UpdatedByNavigation { get; set; }
}
