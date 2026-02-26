using KMS.Repository.Entities;
using KMS.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KMS.Repository.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly ApplicationDbContext _context;

        public InvoiceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // READ
        // ============================================================

        public async Task<KmsInvoice?> GetByIdAsync(int invoiceId)
        {
            return await _context.KmsInvoices
                .Include(i => i.Student)
                .Include(i => i.Template)
                .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);
        }

        public async Task<KmsInvoice?> GetByIdWithDetailsAsync(int invoiceId)
        {
            return await _context.KmsInvoices
                .Include(i => i.Student)
                .Include(i => i.Template)
                .Include(i => i.KmsInvoiceItems)
                .Include(i => i.KmsPayments)
                    .ThenInclude(p => p.ReceivedByNavigation)
                .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);
        }

        public async Task<KmsInvoice?> GetByInvoiceNumberAsync(string invoiceNumber)
        {
            return await _context.KmsInvoices
                .Include(i => i.Student)
                .FirstOrDefaultAsync(i => i.InvoiceNumber == invoiceNumber);
        }

        public async Task<IEnumerable<KmsInvoice>> GetAllAsync()
        {
            return await _context.KmsInvoices
                .Include(i => i.Student)
                .Include(i => i.Template)
                .Include(i => i.KmsPayments)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<KmsInvoice>> GetByStudentIdAsync(int studentId)
        {
            return await _context.KmsInvoices
                .Include(i => i.Student)
                .Include(i => i.KmsInvoiceItems)
                .Include(i => i.KmsPayments)
                .Where(i => i.StudentId == studentId)
                .OrderByDescending(i => i.MonthYear)
                .ToListAsync();
        }

        public async Task<IEnumerable<KmsInvoice>> GetByStatusAsync(string status)
        {
            return await _context.KmsInvoices
                .Include(i => i.Student)
                .Include(i => i.KmsPayments)
                .Where(i => i.Status == status)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<KmsInvoice>> GetByMonthYearAsync(DateOnly monthYear)
        {
            var firstDay = new DateOnly(monthYear.Year, monthYear.Month, 1);
            return await _context.KmsInvoices
                .Include(i => i.Student)
                .Include(i => i.KmsPayments)
                .Where(i => i.MonthYear == firstDay)
                .OrderBy(i => i.Student.FullName)
                .ToListAsync();
        }

        public async Task<IEnumerable<KmsInvoice>> GetOverdueInvoicesAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            return await _context.KmsInvoices
                .Include(i => i.Student)
                .Include(i => i.KmsPayments)
                .Where(i => (i.Status == "Unpaid" || i.Status == "Partial")
                         && i.DueDate.HasValue
                         && i.DueDate.Value < today)
                .OrderBy(i => i.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<KmsInvoice>> GetFilteredAsync(
            int? studentId, string? status, DateOnly? monthYear, bool? isOverdue)
        {
            var query = _context.KmsInvoices
                .Include(i => i.Student)
                .Include(i => i.Template)
                .Include(i => i.KmsPayments)
                .AsQueryable();

            if (studentId.HasValue)
                query = query.Where(i => i.StudentId == studentId.Value);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(i => i.Status == status);

            if (monthYear.HasValue)
            {
                var firstDay = new DateOnly(monthYear.Value.Year, monthYear.Value.Month, 1);
                query = query.Where(i => i.MonthYear == firstDay);
            }

            if (isOverdue == true)
            {
                var today = DateOnly.FromDateTime(DateTime.Today);
                query = query.Where(i => (i.Status == "Unpaid" || i.Status == "Partial")
                                      && i.DueDate.HasValue
                                      && i.DueDate.Value < today);
            }

            return await query.OrderByDescending(i => i.CreatedAt).ToListAsync();
        }

        // ============================================================
        // CREATE
        // ============================================================

        public async Task<KmsInvoice> CreateAsync(KmsInvoice invoice)
        {
            _context.KmsInvoices.Add(invoice);
            await _context.SaveChangesAsync();
            await _context.Entry(invoice).Reference(i => i.Student).LoadAsync();
            await _context.Entry(invoice).Reference(i => i.Template).LoadAsync();
            return invoice;
        }

        public async Task<KmsInvoiceItem> CreateItemAsync(KmsInvoiceItem item)
        {
            _context.KmsInvoiceItems.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        // ============================================================
        // UPDATE
        // ============================================================

        public async Task UpdateAsync(KmsInvoice invoice)
        {
            _context.KmsInvoices.Update(invoice);
            await _context.SaveChangesAsync();
        }

        // ============================================================
        // DELETE
        // ============================================================

        public async Task DeleteAsync(int invoiceId)
        {
            var invoice = await _context.KmsInvoices.FindAsync(invoiceId);
            if (invoice != null)
            {
                _context.KmsInvoices.Remove(invoice);
                await _context.SaveChangesAsync();
            }
        }

        // ============================================================
        // CHECKS & HELPERS
        // ============================================================

        public async Task<bool> InvoiceNumberExistsAsync(string invoiceNumber)
        {
            return await _context.KmsInvoices
                .AnyAsync(i => i.InvoiceNumber == invoiceNumber);
        }

        public async Task<bool> InvoiceExistsForStudentMonthAsync(int studentId, DateOnly monthYear)
        {
            var firstDay = new DateOnly(monthYear.Year, monthYear.Month, 1);
            return await _context.KmsInvoices
                .AnyAsync(i => i.StudentId == studentId && i.MonthYear == firstDay);
        }

        public async Task<string> GenerateInvoiceNumberAsync()
        {
            var prefix = $"INV-{DateTime.Now:yyyyMM}-";
            var lastInvoice = await _context.KmsInvoices
                .Where(i => i.InvoiceNumber.StartsWith(prefix))
                .OrderByDescending(i => i.InvoiceNumber)
                .FirstOrDefaultAsync();

            int nextSeq = 1;
            if (lastInvoice != null)
            {
                var parts = lastInvoice.InvoiceNumber.Split('-');
                if (parts.Length == 3 && int.TryParse(parts[2], out int lastSeq))
                    nextSeq = lastSeq + 1;
            }

            return $"{prefix}{nextSeq:D3}";
        }

        // ============================================================
        // STATS
        // ============================================================

        public async Task<decimal> GetTotalPaidByStudentAsync(int studentId)
        {
            return await _context.KmsPayments
                .Where(p => p.Invoice.StudentId == studentId)
                .SumAsync(p => p.PaidAmount);
        }

        public async Task<decimal> GetTotalUnpaidByStudentAsync(int studentId)
        {
            return await _context.KmsInvoices
                .Where(i => i.StudentId == studentId
                         && (i.Status == "Unpaid" || i.Status == "Partial"))
                .SumAsync(i => i.FinalAmount);
        }
    }
}