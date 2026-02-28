using KMS.Repository.Entities;
using KMS.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Repository.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ApplicationDbContext _context;

        public PaymentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // READ
        // ============================================================

        public async Task<KmsPayment?> GetByIdAsync(int paymentId)
        {
            return await _context.KmsPayments
                .Include(p => p.Invoice)
                    .ThenInclude(i => i.Student)
                .Include(p => p.ReceivedByNavigation)
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId);
        }

        public async Task<KmsPayment?> GetByPaymentNumberAsync(string paymentNumber)
        {
            return await _context.KmsPayments
                .Include(p => p.Invoice)
                    .ThenInclude(i => i.Student)
                .FirstOrDefaultAsync(p => p.PaymentNumber == paymentNumber);
        }

        public async Task<IEnumerable<KmsPayment>> GetAllAsync()
        {
            return await _context.KmsPayments
                .Include(p => p.Invoice)
                    .ThenInclude(i => i.Student)
                .Include(p => p.ReceivedByNavigation)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<KmsPayment>> GetByInvoiceIdAsync(int invoiceId)
        {
            return await _context.KmsPayments
                .Include(p => p.Invoice)
                .Include(p => p.ReceivedByNavigation)
                .Where(p => p.InvoiceId == invoiceId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<KmsPayment>> GetFilteredAsync(
            int? invoiceId, string? paymentMethod, DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.KmsPayments
                .Include(p => p.Invoice)
                    .ThenInclude(i => i.Student)
                .Include(p => p.ReceivedByNavigation)
                .AsQueryable();

            if (invoiceId.HasValue)
                query = query.Where(p => p.InvoiceId == invoiceId.Value);

            if (!string.IsNullOrEmpty(paymentMethod))
                query = query.Where(p => p.PaymentMethod == paymentMethod);

            if (fromDate.HasValue)
                query = query.Where(p => p.PaymentDate >= DateOnly.FromDateTime(fromDate.Value));

            if (toDate.HasValue)
                query = query.Where(p => p.PaymentDate <= DateOnly.FromDateTime(toDate.Value));

            return await query.OrderByDescending(p => p.PaymentDate).ToListAsync();
        }

        public async Task<IEnumerable<KmsPayment>> GetPaymentsByDateRangeAsync(DateTime from, DateTime to)
        {
            return await _context.KmsPayments
                .Include(p => p.Invoice)
                    .ThenInclude(i => i.Student)
                .Where(p => p.PaymentDate >= DateOnly.FromDateTime(from)
                         && p.PaymentDate <= DateOnly.FromDateTime(to))
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        // ============================================================
        // CREATE
        // ============================================================

        public async Task<KmsPayment> CreateAsync(KmsPayment payment)
        {
            _context.KmsPayments.Add(payment);
            await _context.SaveChangesAsync();
            await _context.Entry(payment).Reference(p => p.Invoice).LoadAsync();
            await _context.Entry(payment).Reference(p => p.ReceivedByNavigation).LoadAsync();
            return payment;
        }

        // ============================================================
        // DELETE
        // ============================================================

        public async Task DeleteAsync(int paymentId)
        {
            var payment = await _context.KmsPayments.FindAsync(paymentId);
            if (payment != null)
            {
                _context.KmsPayments.Remove(payment);
                await _context.SaveChangesAsync();
            }
        }

        // ============================================================
        // CHECKS & HELPERS
        // ============================================================

        public async Task<string> GeneratePaymentNumberAsync()
        {
            // Format: PAY-YYYYMM-XXX
            var prefix = $"PAY-{DateTime.Now:yyyyMM}-";
            var lastPayment = await _context.KmsPayments
                .Where(p => p.PaymentNumber != null && p.PaymentNumber.StartsWith(prefix))
                .OrderByDescending(p => p.PaymentNumber)
                .FirstOrDefaultAsync();

            int nextSeq = 1;
            if (lastPayment?.PaymentNumber != null)
            {
                var parts = lastPayment.PaymentNumber.Split('-');
                if (parts.Length == 3 && int.TryParse(parts[2], out int lastSeq))
                    nextSeq = lastSeq + 1;
            }

            return $"{prefix}{nextSeq:D3}";
        }

        public async Task<decimal> GetTotalPaidForInvoiceAsync(int invoiceId)
        {
            return await _context.KmsPayments
                .Where(p => p.InvoiceId == invoiceId)
                .SumAsync(p => p.PaidAmount);
        }

        // ============================================================
        // STATS
        // ============================================================

        public async Task<decimal> GetTotalRevenueAsync(DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.KmsPayments.AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(p => p.PaymentDate >= DateOnly.FromDateTime(fromDate.Value));

            if (toDate.HasValue)
                query = query.Where(p => p.PaymentDate <= DateOnly.FromDateTime(toDate.Value));

            return await query.SumAsync(p => p.PaidAmount);
        }
    }
}
