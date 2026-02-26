using KMS.Service.DTOs;
using KMS.Service.DTOs.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.Interfaces
{
    public interface IPaymentService
    {
        Task<BaseResponseDTO> GetAllPaymentsAsync();
        Task<BaseResponseDTO> GetPaymentByIdAsync(int paymentId);
        Task<BaseResponseDTO> GetPaymentsByInvoiceAsync(int invoiceId);
        Task<BaseResponseDTO> GetFilteredPaymentsAsync(PaymentFilterDTO filter);
        Task<BaseResponseDTO> CreatePaymentAsync(CreatePaymentDTO dto);
        Task<BaseResponseDTO> DeletePaymentAsync(int paymentId);
        Task<BaseResponseDTO> GetRevenueSummaryAsync(DateTime? fromDate, DateTime? toDate);
    }
}
