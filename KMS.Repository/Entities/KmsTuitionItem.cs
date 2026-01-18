using System;
using System.Collections.Generic;

namespace KMS.Repository.Entities;

public partial class KmsTuitionItem
{
    public int ItemId { get; set; }

    public int TemplateId { get; set; }

    public string ItemName { get; set; } = null!;

    public decimal Amount { get; set; }

    public int? ItemOrder { get; set; }

    public virtual KmsTuitionTemplate Template { get; set; } = null!;
}
