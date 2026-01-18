using System;
using System.Collections.Generic;

namespace KMS.Repository.Entities;

public partial class KmsTuitionTemplate
{
    public int TemplateId { get; set; }

    public string TemplateName { get; set; } = null!;

    public string? Description { get; set; }

    public decimal BaseAmount { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public virtual KmsUser? CreatedByNavigation { get; set; }

    public virtual ICollection<KmsInvoice> KmsInvoices { get; set; } = new List<KmsInvoice>();

    public virtual ICollection<KmsTuitionItem> KmsTuitionItems { get; set; } = new List<KmsTuitionItem>();
}
