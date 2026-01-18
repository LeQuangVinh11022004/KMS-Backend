using System;
using System.Collections.Generic;

namespace KMS.Repository.Entities;

public partial class KmsAnnouncement
{
    public int AnnouncementId { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string TargetAudience { get; set; } = null!;

    public int? TargetClassId { get; set; }

    public string? Priority { get; set; }

    public bool? IsPublished { get; set; }

    public DateTime? PublishedAt { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual KmsUser CreatedByNavigation { get; set; } = null!;

    public virtual KmsClass? TargetClass { get; set; }

    public virtual KmsUser? UpdatedByNavigation { get; set; }
}
