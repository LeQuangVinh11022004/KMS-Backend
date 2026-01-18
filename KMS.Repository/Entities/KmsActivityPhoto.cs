using System;
using System.Collections.Generic;

namespace KMS.Repository.Entities;

public partial class KmsActivityPhoto
{
    public int PhotoId { get; set; }

    public int ActivityId { get; set; }

    public string PhotoUrl { get; set; } = null!;

    public string? Caption { get; set; }

    public DateTime? UploadedAt { get; set; }

    public virtual KmsClassActivity Activity { get; set; } = null!;
}
