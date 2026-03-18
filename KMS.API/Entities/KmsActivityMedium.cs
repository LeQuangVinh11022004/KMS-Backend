using System;
using System.Collections.Generic;

namespace KMS.API.Entities;

public partial class KmsActivityMedium
{
    public int MediaId { get; set; }

    public int ActivityId { get; set; }

    public string MediaUrl { get; set; } = null!;

    public string? MediaType { get; set; }

    public string? Caption { get; set; }

    public DateTime? UploadedAt { get; set; }

    public virtual KmsClassActivity Activity { get; set; } = null!;
}
