using System;
using System.Collections.Generic;

namespace KMS.Repository.Entities;

public partial class KmsParentRegistration
{
    public int RegistrationId { get; set; }

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public string FullName { get; set; } = null!;

    public string? ChildFullName { get; set; }

    public DateOnly? ChildDateOfBirth { get; set; }

    public string? RequestMessage { get; set; }

    public string? Status { get; set; }

    public int? ReviewedBy { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public string? ReviewNote { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual KmsUser? ReviewedByNavigation { get; set; }
}
