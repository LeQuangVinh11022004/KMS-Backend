using System;
using System.Collections.Generic;

namespace KMS.Repository.Entities;

public partial class KmsStudentParent
{
    public int StudentId { get; set; }

    public int ParentId { get; set; }

    public string Relationship { get; set; } = null!;

    public bool? IsPrimaryContact { get; set; }

    public DateTime? AssignedAt { get; set; }

    public virtual KmsParent Parent { get; set; } = null!;

    public virtual KmsStudent Student { get; set; } = null!;
}
