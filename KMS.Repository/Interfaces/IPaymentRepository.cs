using KMS.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Repository.Interfaces
{
    public interface IPaymentRepository
    {
        // Read
        Task<KmsPayment?> GetByIdAsync(int paymentId);
        Task<KmsPayment?> GetByPaymentNumberAsync(string paymentNumber);
        Task<IEnumerable<KmsPayment>> GetAllAsync();
        Task<IEnumerable<KmsPayment>> GetByInvoiceIdAsync(int invoiceId);
        Task<IEnumerable<KmsPayment>> GetFilteredAsync(int? invoiceId, string? paymentMethod, DateTime? fromDate, DateTime? toDate);

        // Create
        Task<KmsPayment> CreateAsync(KmsPayment payment);

        // Delete (payments generally shouldn't be updated - only deleted and re-entered)
        Task DeleteAsync(int paymentId);

        // Checks & Helpers
        Task<string> GeneratePaymentNumberAsync(); // Auto generate PAY-YYYYMM-XXX
        Task<decimal> GetTotalPaidForInvoiceAsync(int invoiceId);

        // Stats
        Task<decimal> GetTotalRevenueAsync(DateTime? fromDate, DateTime? toDate);
        Task<IEnumerable<KmsPayment>> GetPaymentsByDateRangeAsync(DateTime from, DateTime to);
    }
}
