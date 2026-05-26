using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.DTOs.VNPay
{
    public class CreateVNPayPaymentDTO
    {
        /// <summary>ID của Invoice cần thanh toán</summary>
        [DefaultValue(1)]
        public int InvoiceId { get; set; }

        /// <summary>Nội dung thanh toán</summary>
        [DefaultValue("Thanh toan hoc phi")]
        public string OrderInfo { get; set; } = "Thanh toan hoc phi";

        /// <summary>Để trống = hiện danh sách | VNBANK = ATM | VNPAYQR = QR</summary>
        [DefaultValue("")]
        public string? BankCode { get; set; }

        /// <summary>URL frontend để VNPay chuyển về sau khi thanh toán</summary>

        [DefaultValue("")]
        public string? ReturnUrl { get; set; } = null;
    }

    public class VNPayPaymentUrlDTO
    {
        public string PaymentUrl { get; set; } = null!;
        public string OrderId { get; set; } = null!;
        public decimal Amount { get; set; }
    }

    public class VNPayCallbackResultDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; } = null!;
        public string? VnpTransactionNo { get; set; }
        public string? OrderId { get; set; }
        public decimal? Amount { get; set; }
        public string? BankCode { get; set; }
        public int? InvoiceId { get; set; }
        public string? InvoiceStatus { get; set; }
        public int? PaymentId { get; set; }
    }
}
