using System;
using System.Collections.Generic;

namespace KMS.Repository.Entities;

public partial class KmsClassStudent
{
    public int ClassId { get; set; }

    public int StudentId { get; set; }

    public DateTime? EnrolledAt { get; set; }

    public int? EnrolledBy { get; set; }

    public virtual KmsClass Class { get; set; } = null!;

    public virtual KmsUser? EnrolledByNavigation { get; set; }

    public virtual KmsStudent Student { get; set; } = null!;
}
