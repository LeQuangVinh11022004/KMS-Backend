using System;
using System.Collections.Generic;

namespace KMS.Repository.Entities;

public partial class KmsClassActivity
{
    public int ActivityId { get; set; }

    public int ClassId { get; set; }

    public string Title { get; set; } = null!;

    public string? Content { get; set; }

    public DateOnly? ActivityDate { get; set; }

    public int PostedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual KmsClass Class { get; set; } = null!;

    public virtual ICollection<KmsActivityPhoto> KmsActivityPhotos { get; set; } = new List<KmsActivityPhoto>();

    public virtual KmsTeacher PostedByNavigation { get; set; } = null!;

    public virtual KmsTeacher? UpdatedByNavigation { get; set; }
}
