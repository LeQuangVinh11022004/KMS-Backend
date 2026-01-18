using System;
using System.Collections.Generic;

namespace KMS.Repository.Entities;

public partial class KmsParent
{
    public int ParentId { get; set; }

    public int UserId { get; set; }

    public string? Occupation { get; set; }

    public string? WorkAddress { get; set; }

    public string? EmergencyContact { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<KmsStudentParent> KmsStudentParents { get; set; } = new List<KmsStudentParent>();

    public virtual KmsUser User { get; set; } = null!;
}
