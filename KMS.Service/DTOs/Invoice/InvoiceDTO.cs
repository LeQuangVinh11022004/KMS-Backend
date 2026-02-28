using KMS.Service.DTOs.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.DTOs.Invoice
{
    public class InvoiceDTO
    {
        public int InvoiceId { get; set; }
        public string InvoiceNumber { get; set; } = null!;
        public int StudentId { get; set; }
        public string StudentName { get; set; } = null!;
        public string? StudentCode { get; set; }
        public int? TemplateId { get; set; }
        public string? TemplateName { get; set; }
        public DateTime MonthYear { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public string Status { get; set; } = null!;
        public DateTime? DueDate { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<InvoiceItemDTO> Items { get; set; } = new();
        public List<PaymentDTO> Payments { get; set; } = new();
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }
    }

    public class InvoiceItemDTO
    {
        public int InvoiceItemId { get; set; }
        public int InvoiceId { get; set; }
        public string ItemName { get; set; } = null!;
        public decimal Amount { get; set; }
    }

    public class CreateInvoiceDTO
    {
        public int StudentId { get; set; }
        public int? TemplateId { get; set; }
        public DateTime MonthYear { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DiscountAmount { get; set; } = 0;
        public DateTime? DueDate { get; set; }
        public string? Notes { get; set; }
        public List<CreateInvoiceItemDTO> Items { get; set; } = new();
    }

    public class CreateInvoiceItemDTO
    {
        public string ItemName { get; set; } = null!;
        public decimal Amount { get; set; }
    }

    public class UpdateInvoiceDTO
    {
        public decimal? DiscountAmount { get; set; }
        public string? Status { get; set; }
        public DateTime? DueDate { get; set; }
        public string? Notes { get; set; }
    }

    public class InvoiceFilterDTO
    {
        public int? StudentId { get; set; }
        public string? Status { get; set; }
        public DateTime? MonthYear { get; set; }
        public bool? IsOverdue { get; set; }
    }

}
