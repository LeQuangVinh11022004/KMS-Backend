using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.DTOs.TuitionTemplate
{
    public class TuitionTemplateDTO
    {
        public int TemplateId { get; set; }
        public string TemplateName { get; set; } = null!;
        public string? Description { get; set; }
        public decimal BaseAmount { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<TuitionItemDTO> Items { get; set; } = new();
    }

    public class TuitionItemDTO
    {
        public int ItemId { get; set; }
        public int TemplateId { get; set; }
        public string ItemName { get; set; } = null!;
        public decimal Amount { get; set; }
        public int ItemOrder { get; set; }
    }

    public class CreateTuitionTemplateDTO
    {
        public string TemplateName { get; set; } = null!;
        public string? Description { get; set; }
        public decimal BaseAmount { get; set; }
        public List<CreateTuitionItemDTO> Items { get; set; } = new();
    }

    public class CreateTuitionItemDTO
    {
        public string ItemName { get; set; } = null!;
        public decimal Amount { get; set; }
        public int ItemOrder { get; set; } = 1;
    }

    public class UpdateTuitionTemplateDTO
    {
        public string? TemplateName { get; set; }
        public string? Description { get; set; }
        public decimal? BaseAmount { get; set; }
        public bool? IsActive { get; set; }
        public List<CreateTuitionItemDTO>? Items { get; set; } // Replace all items if provided
    }
}
