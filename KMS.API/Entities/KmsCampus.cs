using System;
using System.Collections.Generic;

namespace KMS.API.Entities;

public partial class KmsCampus
{
    public int CampusId { get; set; }

    public string CampusName { get; set; } = null!;

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<KmsClass> KmsClasses { get; set; } = new List<KmsClass>();

    public virtual ICollection<KmsStudent> KmsStudents { get; set; } = new List<KmsStudent>();

    public virtual ICollection<KmsTeacher> KmsTeachers { get; set; } = new List<KmsTeacher>();

    public virtual ICollection<KmsUser> KmsUsers { get; set; } = new List<KmsUser>();
}
