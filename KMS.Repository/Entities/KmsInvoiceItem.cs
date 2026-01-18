using System;
using System.Collections.Generic;

namespace KMS.Repository.Entities;

public partial class KmsInvoiceItem
{
    public int InvoiceItemId { get; set; }

    public int InvoiceId { get; set; }

    public string ItemName { get; set; } = null!;

    public decimal Amount { get; set; }

    public virtual KmsInvoice Invoice { get; set; } = null!;
}
