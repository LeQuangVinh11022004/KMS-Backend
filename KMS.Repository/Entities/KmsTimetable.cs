using System;
using System.Collections.Generic;

namespace KMS.Repository.Entities;

public partial class KmsTimetable
{
    public int TimetableId { get; set; }

    public int ClassId { get; set; }

    public int DayOfWeek { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public string Subject { get; set; } = null!;

    public int? TeacherId { get; set; }

    public string? Room { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public virtual KmsClass Class { get; set; } = null!;

    public virtual KmsUser? CreatedByNavigation { get; set; }

    public virtual KmsTeacher? Teacher { get; set; }
}
