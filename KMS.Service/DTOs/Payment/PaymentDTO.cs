using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.DTOs.Payment
{
    public class PaymentDTO
    {
        public int PaymentId { get; set; }
        public string? PaymentNumber { get; set; }
        public int InvoiceId { get; set; }
        public string InvoiceNumber { get; set; } = null!;
        public string StudentName { get; set; } = null!;
        public decimal PaidAmount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public string? TransactionReference { get; set; }
        public string? Notes { get; set; }
        public int ReceivedBy { get; set; }
        public string ReceivedByName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }

    public class CreatePaymentDTO
    {
        public int InvoiceId { get; set; }
        public decimal PaidAmount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public string? TransactionReference { get; set; }
        public string? Notes { get; set; }
        public int ReceivedBy { get; set; }
    }

    public class PaymentFilterDTO
    {
        public int? InvoiceId { get; set; }
        public string? PaymentMethod { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
