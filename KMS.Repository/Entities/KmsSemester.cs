using System;
using System.Collections.Generic;

namespace KMS.Repository.Entities;

public partial class KmsSemester
{
    public int SemesterId { get; set; }

    public int SchoolYearId { get; set; }

    public string SemesterName { get; set; } = null!;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public bool? IsActive { get; set; }

    public virtual KmsSchoolYear SchoolYear { get; set; } = null!;
}
