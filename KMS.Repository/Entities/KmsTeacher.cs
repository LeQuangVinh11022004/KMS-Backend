using System;
using System.Collections.Generic;

namespace KMS.Repository.Entities;

public partial class KmsTeacher
{
    public int TeacherId { get; set; }

    public int UserId { get; set; }

    public string? TeacherCode { get; set; }

    public string? Specialization { get; set; }

    public DateOnly? HireDate { get; set; }

    public decimal? Salary { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<KmsAttendance> KmsAttendanceTakenByNavigations { get; set; } = new List<KmsAttendance>();

    public virtual ICollection<KmsAttendance> KmsAttendanceUpdatedByNavigations { get; set; } = new List<KmsAttendance>();

    public virtual ICollection<KmsClassActivity> KmsClassActivityPostedByNavigations { get; set; } = new List<KmsClassActivity>();

    public virtual ICollection<KmsClassActivity> KmsClassActivityUpdatedByNavigations { get; set; } = new List<KmsClassActivity>();

    public virtual ICollection<KmsClassTeacher> KmsClassTeachers { get; set; } = new List<KmsClassTeacher>();

    public virtual ICollection<KmsTimetable> KmsTimetables { get; set; } = new List<KmsTimetable>();

    public virtual KmsUser User { get; set; } = null!;
}
