using System;
using System.Collections.Generic;

namespace KMS.Repository.Entities;

public partial class KmsSchoolYear
{
    public int SchoolYearId { get; set; }

    public string YearName { get; set; } = null!;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public virtual ICollection<KmsClass> KmsClasses { get; set; } = new List<KmsClass>();

    public virtual ICollection<KmsSemester> KmsSemesters { get; set; } = new List<KmsSemester>();
}
