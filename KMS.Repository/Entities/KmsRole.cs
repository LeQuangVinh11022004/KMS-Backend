using System;
using System.Collections.Generic;

namespace KMS.Repository.Entities;

public partial class KmsRole
{
    public int RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<KmsUserRole> KmsUserRoles { get; set; } = new List<KmsUserRole>();
}
