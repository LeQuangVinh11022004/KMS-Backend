using System;
using System.Collections.Generic;

namespace KMS.Repository.Entities;

public partial class KmsClassTeacher
{
    public int ClassId { get; set; }

    public int TeacherId { get; set; }

    public string? Role { get; set; }

    public DateTime? AssignedAt { get; set; }

    public int? AssignedBy { get; set; }

    public virtual KmsUser? AssignedByNavigation { get; set; }

    public virtual KmsClass Class { get; set; } = null!;

    public virtual KmsTeacher Teacher { get; set; } = null!;
}
