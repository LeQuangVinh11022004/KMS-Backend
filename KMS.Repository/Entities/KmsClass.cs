using System;
using System.Collections.Generic;

namespace KMS.Repository.Entities;

public partial class KmsClass
{
    public int ClassId { get; set; }

    public string ClassName { get; set; } = null!;

    public int SchoolYearId { get; set; }

    public string? AgeGroup { get; set; }

    public int? MaxCapacity { get; set; }

    public int? CurrentEnrollment { get; set; }

    public string? Room { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public virtual KmsUser? CreatedByNavigation { get; set; }

    public virtual ICollection<KmsAnnouncement> KmsAnnouncements { get; set; } = new List<KmsAnnouncement>();

    public virtual ICollection<KmsAttendance> KmsAttendances { get; set; } = new List<KmsAttendance>();

    public virtual ICollection<KmsClassActivity> KmsClassActivities { get; set; } = new List<KmsClassActivity>();

    public virtual ICollection<KmsClassStudent> KmsClassStudents { get; set; } = new List<KmsClassStudent>();

    public virtual ICollection<KmsClassTeacher> KmsClassTeachers { get; set; } = new List<KmsClassTeacher>();

    public virtual ICollection<KmsMenu> KmsMenus { get; set; } = new List<KmsMenu>();

    public virtual ICollection<KmsTimetable> KmsTimetables { get; set; } = new List<KmsTimetable>();

    public virtual KmsSchoolYear SchoolYear { get; set; } = null!;
}
