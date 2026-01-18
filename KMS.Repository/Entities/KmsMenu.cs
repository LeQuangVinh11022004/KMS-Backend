using System;
using System.Collections.Generic;

namespace KMS.Repository.Entities;

public partial class KmsMenu
{
    public int MenuId { get; set; }

    public int? ClassId { get; set; }

    public DateOnly MenuDate { get; set; }

    public string MealType { get; set; } = null!;

    public string MenuContent { get; set; } = null!;

    public int? Calories { get; set; }

    public string? Allergens { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual KmsClass? Class { get; set; }

    public virtual KmsUser? CreatedByNavigation { get; set; }

    public virtual KmsUser? UpdatedByNavigation { get; set; }
}
