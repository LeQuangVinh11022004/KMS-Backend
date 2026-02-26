using KMS.Service.DTOs;
using KMS.Service.DTOs.Invoice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.Interfaces
{
    public interface IInvoiceService
    {
        Task<BaseResponseDTO> GetAllInvoicesAsync();
        Task<BaseResponseDTO> GetInvoiceByIdAsync(int invoiceId);
        Task<BaseResponseDTO> GetInvoicesByStudentAsync(int studentId);
        Task<BaseResponseDTO> GetFilteredInvoicesAsync(InvoiceFilterDTO filter);
        Task<BaseResponseDTO> GetOverdueInvoicesAsync();
        Task<BaseResponseDTO> CreateInvoiceAsync(CreateInvoiceDTO dto, int createdBy);
        Task<BaseResponseDTO> CreateInvoiceFromTemplateAsync(int studentId, int templateId, DateTime monthYear, int createdBy);
        Task<BaseResponseDTO> UpdateInvoiceAsync(int invoiceId, UpdateInvoiceDTO dto, int updatedBy);
        Task<BaseResponseDTO> DeleteInvoiceAsync(int invoiceId);
        Task<BaseResponseDTO> MarkOverdueInvoicesAsync(); // Batch update expired unpaid invoices
    }
}
