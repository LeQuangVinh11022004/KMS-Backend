using System;
using System.Collections.Generic;

namespace KMS.API.Entities;

public partial class KmsHealthCheck
{
    public int CheckId { get; set; }

    public int StudentId { get; set; }

    public DateOnly? CheckDate { get; set; }

    public decimal? Height { get; set; }

    public decimal? Weight { get; set; }

    public string? EyeSight { get; set; }

    public string? DentalStatus { get; set; }

    public string? Note { get; set; }

    public int? CheckedBy { get; set; }

    public virtual KmsStudent Student { get; set; } = null!;
}
