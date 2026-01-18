using System;
using System.Collections.Generic;

namespace KMS.Repository.Entities;

public partial class KmsAttendance
{
    public int AttendanceId { get; set; }

    public int StudentId { get; set; }

    public int ClassId { get; set; }

    public DateOnly AttendanceDate { get; set; }

    public string Status { get; set; } = null!;

    public TimeOnly? CheckInTime { get; set; }

    public TimeOnly? CheckOutTime { get; set; }

    public string? Notes { get; set; }

    public int TakenBy { get; set; }

    public DateTime? TakenAt { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual KmsClass Class { get; set; } = null!;

    public virtual KmsStudent Student { get; set; } = null!;

    public virtual KmsTeacher TakenByNavigation { get; set; } = null!;

    public virtual KmsTeacher? UpdatedByNavigation { get; set; }
}
