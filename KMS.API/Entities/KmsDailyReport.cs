using System;
using System.Collections.Generic;

namespace KMS.API.Entities;

public partial class KmsDailyReport
{
    public int ReportId { get; set; }

    public int StudentId { get; set; }

    public int ClassId { get; set; }

    public DateOnly? ReportDate { get; set; }

    public string? LivingData { get; set; }

    public int CreatedBy { get; set; }

    public string? EducationalData { get; set; }

    public int? ReviewedBy { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual KmsClass Class { get; set; } = null!;

    public virtual KmsUser CreatedByNavigation { get; set; } = null!;

    public virtual KmsUser? ReviewedByNavigation { get; set; }

    public virtual KmsStudent Student { get; set; } = null!;
}
