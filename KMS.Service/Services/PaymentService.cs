using KMS.Repository.Entities;
using KMS.Repository.Interfaces;
using KMS.Service.DTOs;
using KMS.Service.DTOs.Payment;
using KMS.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IInvoiceRepository _invoiceRepository;

        public PaymentService(IPaymentRepository paymentRepository, IInvoiceRepository invoiceRepository)
        {
            _paymentRepository = paymentRepository;
            _invoiceRepository = invoiceRepository;
        }

        // ============================================================
        // READ
        // ============================================================

        public async Task<BaseResponseDTO> GetAllPaymentsAsync()
        {
            try
            {
                var payments = await _paymentRepository.GetAllAsync();
                var dtos = payments.Select(MapToDTO).ToList();

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"Retrieved {dtos.Count} payments",
                    Data = dtos
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<BaseResponseDTO> GetPaymentByIdAsync(int paymentId)
        {
            try
            {
                var payment = await _paymentRepository.GetByIdAsync(paymentId);
                if (payment == null)
                    return new BaseResponseDTO { Success = false, Message = "Payment not found" };

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = "Payment retrieved successfully",
                    Data = MapToDTO(payment)
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<BaseResponseDTO> GetPaymentsByInvoiceAsync(int invoiceId)
        {
            try
            {
                var invoice = await _invoiceRepository.GetByIdAsync(invoiceId);
                if (invoice == null)
                    return new BaseResponseDTO { Success = false, Message = "Invoice not found" };

                var payments = await _paymentRepository.GetByInvoiceIdAsync(invoiceId);
                var dtos = payments.Select(MapToDTO).ToList();

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"Retrieved {dtos.Count} payments for invoice",
                    Data = dtos
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<BaseResponseDTO> GetFilteredPaymentsAsync(PaymentFilterDTO filter)
        {
            try
            {
                var payments = await _paymentRepository.GetFilteredAsync(
                    filter.InvoiceId, filter.PaymentMethod, filter.FromDate, filter.ToDate);
                var dtos = payments.Select(MapToDTO).ToList();

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"Found {dtos.Count} payments",
                    Data = dtos
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        // ============================================================
        // CREATE
        // ============================================================

        public async Task<BaseResponseDTO> CreatePaymentAsync(CreatePaymentDTO dto)
        {
            try
            {
                var invoice = await _invoiceRepository.GetByIdWithDetailsAsync(dto.InvoiceId);
                if (invoice == null)
                    return new BaseResponseDTO { Success = false, Message = "Invoice not found" };

                if (invoice.Status == "Paid")
                    return new BaseResponseDTO { Success = false, Message = "Invoice is already fully paid" };

                if (invoice.Status == "Cancelled")
                    return new BaseResponseDTO { Success = false, Message = "Cannot pay a cancelled invoice" };

                if (dto.PaidAmount <= 0)
                    return new BaseResponseDTO { Success = false, Message = "Payment amount must be greater than 0" };

                // Check if overpaying
                var alreadyPaid = await _paymentRepository.GetTotalPaidForInvoiceAsync(dto.InvoiceId);
                var remaining = invoice.FinalAmount - alreadyPaid;

                if (dto.PaidAmount > remaining)
                    return new BaseResponseDTO
                    {
                        Success = false,
                        Message = $"Payment amount ({dto.PaidAmount:C}) exceeds remaining balance ({remaining:C})"
                    };

                var paymentNumber = await _paymentRepository.GeneratePaymentNumberAsync();

                var payment = new KmsPayment
                {
                    InvoiceId = dto.InvoiceId,
                    PaymentNumber = paymentNumber,
                    PaidAmount = dto.PaidAmount,
                    PaymentDate = DateOnly.FromDateTime(dto.PaymentDate),
                    PaymentMethod = dto.PaymentMethod,
                    TransactionReference = dto.TransactionReference,
                    Notes = dto.Notes,
                    ReceivedBy = dto.ReceivedBy,
                    CreatedAt = DateTime.Now
                };

                var created = await _paymentRepository.CreateAsync(payment);

                // Auto-update invoice status based on total paid
                var newTotalPaid = alreadyPaid + dto.PaidAmount;
                invoice.Status = newTotalPaid >= invoice.FinalAmount ? "Paid" : "Partial";
                invoice.UpdatedAt = DateTime.Now;
                await _invoiceRepository.UpdateAsync(invoice);

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = $"Payment recorded successfully. Invoice status: {invoice.Status}",
                    Data = MapToDTO(created)
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        // ============================================================
        // DELETE
        // ============================================================

        public async Task<BaseResponseDTO> DeletePaymentAsync(int paymentId)
        {
            try
            {
                var payment = await _paymentRepository.GetByIdAsync(paymentId);
                if (payment == null)
                    return new BaseResponseDTO { Success = false, Message = "Payment not found" };

                var invoiceId = payment.InvoiceId;
                await _paymentRepository.DeleteAsync(paymentId);

                // Recalculate invoice status after removing payment
                var invoice = await _invoiceRepository.GetByIdAsync(invoiceId);
                if (invoice != null)
                {
                    var totalPaid = await _paymentRepository.GetTotalPaidForInvoiceAsync(invoiceId);
                    if (totalPaid <= 0)
                        invoice.Status = "Unpaid";
                    else if (totalPaid < invoice.FinalAmount)
                        invoice.Status = "Partial";
                    else
                        invoice.Status = "Paid";

                    invoice.UpdatedAt = DateTime.Now;
                    await _invoiceRepository.UpdateAsync(invoice);
                }

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = "Payment deleted and invoice status recalculated"
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        // ============================================================
        // REVENUE SUMMARY
        // ============================================================

        public async Task<BaseResponseDTO> GetRevenueSummaryAsync(DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                var from = fromDate ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var to = toDate ?? DateTime.Now;

                var totalRevenue = await _paymentRepository.GetTotalRevenueAsync(from, to);
                var payments = await _paymentRepository.GetPaymentsByDateRangeAsync(from, to);

                var summary = new
                {
                    FromDate = from,
                    ToDate = to,
                    TotalRevenue = totalRevenue,
                    TotalTransactions = payments.Count(),
                    ByMethod = payments
                        .GroupBy(p => p.PaymentMethod)
                        .Select(g => new
                        {
                            PaymentMethod = g.Key,
                            TotalAmount = g.Sum(p => p.PaidAmount),
                            Count = g.Count()
                        }).ToList()
                };

                return new BaseResponseDTO
                {
                    Success = true,
                    Message = "Revenue summary retrieved successfully",
                    Data = summary
                };
            }
            catch (Exception ex)
            {
                return new BaseResponseDTO { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        // ============================================================
        // MAPPER
        // ============================================================

        private PaymentDTO MapToDTO(KmsPayment p)
        {
            return new PaymentDTO
            {
                PaymentId = p.PaymentId,
                PaymentNumber = p.PaymentNumber,
                InvoiceId = p.InvoiceId,
                InvoiceNumber = p.Invoice?.InvoiceNumber ?? "",
                StudentName = p.Invoice?.Student?.FullName ?? "",
                PaidAmount = p.PaidAmount,
                PaymentDate = p.PaymentDate.ToDateTime(TimeOnly.MinValue),
                PaymentMethod = p.PaymentMethod,
                TransactionReference = p.TransactionReference,
                Notes = p.Notes,
                ReceivedBy = p.ReceivedBy,
                ReceivedByName = p.ReceivedByNavigation?.FullName ?? "",
                CreatedAt = p.CreatedAt ?? DateTime.Now
            };
        }
    }
}
