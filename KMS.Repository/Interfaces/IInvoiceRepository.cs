using KMS.Repository.Entities;

namespace KMS.Repository.Interfaces
{
    public interface IInvoiceRepository
    {
        // Read
        Task<KmsInvoice?> GetByIdAsync(int invoiceId);
        Task<KmsInvoice?> GetByIdWithDetailsAsync(int invoiceId);
        Task<KmsInvoice?> GetByInvoiceNumberAsync(string invoiceNumber);
        Task<IEnumerable<KmsInvoice>> GetAllAsync();
        Task<IEnumerable<KmsInvoice>> GetByStudentIdAsync(int studentId);
        Task<IEnumerable<KmsInvoice>> GetByStatusAsync(string status);
        Task<IEnumerable<KmsInvoice>> GetByMonthYearAsync(DateOnly monthYear);
        Task<IEnumerable<KmsInvoice>> GetOverdueInvoicesAsync();
        Task<IEnumerable<KmsInvoice>> GetFilteredAsync(int? studentId, string? status, DateOnly? monthYear, bool? isOverdue);

        // Create
        Task<KmsInvoice> CreateAsync(KmsInvoice invoice);
        Task<KmsInvoiceItem> CreateItemAsync(KmsInvoiceItem item);

        // Update
        Task UpdateAsync(KmsInvoice invoice);

        // Delete
        Task DeleteAsync(int invoiceId);

        // Checks & Helpers
        Task<bool> InvoiceNumberExistsAsync(string invoiceNumber);
        Task<bool> InvoiceExistsForStudentMonthAsync(int studentId, DateOnly monthYear);
        Task<string> GenerateInvoiceNumberAsync();

        // Stats
        Task<decimal> GetTotalPaidByStudentAsync(int studentId);
        Task<decimal> GetTotalUnpaidByStudentAsync(int studentId);
    }
}